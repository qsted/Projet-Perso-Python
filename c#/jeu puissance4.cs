using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using projet;
using System.Threading;
using System.Collections;

namespace projet
{
    //游戏的结果
    public enum Resultat { j1gagne, j0gagne, partieNulle, indetermine }

    //一个棋牌游戏的当前状态，棋盘状态
    public abstract class Position
    {
        public bool j1aletrait;
        public Position(bool j1aletrait) { this.j1aletrait = j1aletrait; }
        public virtual Resultat Eval { get; protected set; }
        public int NbCoups { get; protected set; }
        public abstract void EffectuerCoup(int i);
        public abstract Position Clone();
        public abstract void Affiche();
    }

    //抽象类 玩家
    public abstract class Joueur
    {
        public abstract int Jouer(Position p);
        public virtual void NouvellePartie() { }
    }

    //一个对局，从开始到结束的完整对局
    public class Partie
    {
        Position pCourante;
        Joueur j1, j0;
        public Resultat r;

        public Partie(Joueur j1, Joueur j0, Position pInitiale)
        {
            this.j1 = j1;
            this.j0 = j0;
            pCourante = pInitiale.Clone();
        }

        public void NouveauMatch(Position pInitiale)
        {
            pCourante = pInitiale.Clone();
        }

        public void Commencer(bool affichage = true)
        {
            j1.NouvellePartie();
            j0.NouvellePartie();
            do
            {
                if (affichage) pCourante.Affiche();
                if (pCourante.j1aletrait)
                {
                    pCourante.EffectuerCoup(j1.Jouer(pCourante.Clone()));
                }
                else
                {
                    pCourante.EffectuerCoup(j0.Jouer(pCourante.Clone()));
                }
            } while (pCourante.NbCoups > 0);
            r = pCourante.Eval;
            if (affichage)
            {
                pCourante.Affiche();
                switch (r)
                {
                    case Resultat.j1gagne: Console.WriteLine("j1 {0} a gagné.", j1); break;
                    case Resultat.j0gagne: Console.WriteLine("j0 {0} a gagné.", j0); break;
                    case Resultat.partieNulle: Console.WriteLine("Partie nulle."); break;
                }
            }
        }
    }

    //一个节点
    public class Noeud
    {
        static Random gen = new Random();

        public Position p;
        public Noeud pere;
        public Noeud[] fils;
        public int cross, win;
        public int indiceMeilleurFils;

        public Noeud(Noeud pere, Position p)
        {
            this.pere = pere;
            this.p = p;
            fils = new Noeud[this.p.NbCoups];
        }

        //选取最好的子节点
        //fils是当前类中的一个array
        public void CalculMeilleurFils(Func<int, int, float> phi)
        {
            float s;
            float sM = 0;
            if (p.j1aletrait)
            {
                //自己的回合 选取胜率最大的 子节点
                for (int i = 0; i < fils.Length; i++)
                {
                    if (fils[i] == null) { s = phi(0, 0); }
                    else { s = phi(fils[i].win, fils[i].cross); }
                    if (s > sM) { sM = s; indiceMeilleurFils = i; }
                }
            }
            else
            {
                //对手的回合 选取胜率最小的 子节点
                for (int i = 0; i < fils.Length; i++)
                {
                    if (fils[i] == null) { s = phi(0, 0); }
                    else { s = phi(fils[i].cross - fils[i].win, fils[i].cross); }
                    if (s > sM) { sM = s; indiceMeilleurFils = i; }
                }
            }
        }


        public Noeud MeilleurFils()
        {
            if (fils[indiceMeilleurFils] != null)
            {
                return fils[indiceMeilleurFils];
            }
            Position q = p.Clone();
            q.EffectuerCoup(indiceMeilleurFils);
            fils[indiceMeilleurFils] = new Noeud(this, q);
            return fils[indiceMeilleurFils];
        }

        public override string ToString()
        {
            string s = "";
            s = s + "indice MF = " + indiceMeilleurFils;
            s += String.Format(" note= {0}\n", fils[indiceMeilleurFils] == null ? "?" : ((1F * fils[indiceMeilleurFils].win) / fils[indiceMeilleurFils].cross).ToString());
            int sc = 0;
            for (int k = 0; k < fils.Length; k++)
            {
                if (fils[k] != null)
                {
                    sc += fils[k].cross;
                    s += (fils[k].win + "/" + fils[k].cross + " ");
                }
                else s += (0 + "/" + 0 + " ");
            }
            s += "\n nbC=" + (sc / 2);
            return s;
        }
    }

    //一个MC的搜索实例
    public class JMCTS : Joueur
    {
        public static Random gen = new Random();
        protected static Stopwatch sw = new Stopwatch();

        protected float a, b;
        protected int temps;

        protected Noeud racine;

        public JMCTS(float a, float b, int temps)
        {
            this.a = 2 * a;
            this.b = 2 * b;
            this.temps = temps;
        }

        public override string ToString()
        {
            return string.Format("JMCTS[{0} - {1} - temps={2}]", a / 2, b / 2, temps);
        }

        public virtual int JeuHasard(Position p)
        {
            Position q = p.Clone();
            int re = 1;
            while (q.NbCoups > 0)
            {
                q.EffectuerCoup(gen.Next(0, q.NbCoups));
            }
            if (q.Eval == Resultat.j1gagne) { re = 2; }
            if (q.Eval == Resultat.j0gagne) { re = 0; }
            return re;
        }


        public override int Jouer(Position p)
        {
            sw.Restart();
            Func<int, int, float> phi = (W, C) => (a + W) / (b + C);

            racine = new Noeud(null, p);
            int iter = 0;
            while (sw.ElapsedMilliseconds < temps)
            {
                Noeud no = racine;

                do // Sélection
                {
                    no.CalculMeilleurFils(phi);
                    no = no.MeilleurFils();

                } while (no.cross > 0 && no.fils.Length > 0);


                int re = JeuHasard(no.p); // Simulation

                while (no != null) // Rétropropagation
                {
                    no.cross += 2;
                    no.win += re;
                    no = no.pere;
                }
                iter++;
            }
            racine.CalculMeilleurFils(phi);

            //Console.WriteLine("{0} itérations", iter);
            //Console.WriteLine(racine);
            return racine.indiceMeilleurFils;
        }
    }
}
//Q1-Q2
namespace allumettes
{
    //介绍 http://maths.amatheurs.fr/index.php?page=allumettes
    //定义一个 allumettes 的游戏
    //包括一个正整数 代表当前的火柴数量
    //展示方法：打印文字
    public class PositionA : projet.Position
    {
        private int NbAllumettes;
        private int NbAllumettesCourante;

        public override Resultat Eval
        {
            get
            {
                if (NbAllumettesCourante == 0)
                {
                    if (this.j1aletrait)
                    { 
                        return Resultat.j0gagne;
                    }
                    else
                    {
                        return Resultat.j1gagne;
                    }
                }
                else {
                    return Resultat.indetermine;
                }
            }
        }

        private int[] range = { 1, 3 };

        //都是些constructor
        public PositionA(bool j1aletrait) : base(j1aletrait)
        {
            this.Eval = Resultat.indetermine;
            this.NbCoups = 3; //总共可以行动的次数，起始是 1 2 3 
        }
        public PositionA(bool j1aletrait, int NbAllumettes) : this(j1aletrait) //调用上面的PositionA(bool j1aletrait)
        {
            this.NbAllumettes = NbAllumettes;
            this.NbAllumettesCourante = NbAllumettes;
        }
        public PositionA(bool j1aletrait, int NbAllumettes, int NbAllumettesCourante) : this(j1aletrait, NbAllumettes)
        {
            this.NbAllumettesCourante = NbAllumettesCourante;
            this.NbCoups = NbAllumettesCourante < 3 ? NbAllumettesCourante : 3; //总共可以行动的次数，最多不超过火柴数量
        }

        //打印当前的游戏状态
        public override void Affiche()
        {
            Console.WriteLine("\n========================\n" +
                "Joueur {0} a le trait,  NbAllumetesCurrent : {1}  ", 
                this.j1aletrait ? 1 : 0, 
                this.NbAllumettesCourante);
        }

        //用来复制一个当前的游戏状态
        public override Position Clone()
        {
            return new PositionA(this.j1aletrait, NbAllumettes, NbAllumettesCourante);
        }

        //做出动作，完成一次游戏，也就是拿走 i 个火柴,  i \in [1,2,3]
        //如果数字不合理 就报错
        //如果数字正常，就从当前的NbAllumettesCurrent 中减去 i 个
        public override void EffectuerCoup(int i)
        {
            i += 1;
            if (i < this.range[0] || i > this.range[1])
            {
                Console.WriteLine("Illegal move, out of range [{0}, {1}].", this.range[0], this.range[1]);
                return;
            }
            if (i > NbAllumettesCourante)
            {
                Console.WriteLine("Illegal move, not enough matches.");
                NbAllumettesCourante = 0;
                return;
            }
            this.NbAllumettesCourante -= i;

            //更新可执行的行动数量
            switch (this.NbAllumettesCourante)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    this.NbCoups = this.NbAllumettesCourante;
                    break;
                default:
                    break;
            }

            //判断游戏是否胜利
            if (this.NbAllumettesCourante == 0)
            {
            }
            else { 
                //更新j1 a le trait;
                this.j1aletrait = !this.j1aletrait;
            }


        }
    }
    public class JoueurHumainA : projet.Joueur
    {
        public override int Jouer(Position p)
        {
            //询问玩家输入 最后把玩家的选择发回去
            //p.Affiche();
            Console.WriteLine("Choose your action from range [0, {0}[ to remove i+1 matches: ", p.NbCoups);
            int intTemp = Convert.ToInt32(Console.ReadLine());
            return intTemp;
        }
    }
    public class TestQ2 {
        public TestQ2()
        {
            PositionA pInitiale = new PositionA(true, 11);
            JoueurHumainA j0 = new JoueurHumainA();
            JMCTS j1 = new JMCTS(10, 10, 100);

            Partie partie = new Partie(j1, j0, pInitiale);
            partie.Commencer(true);
            Console.ReadLine();
        }
    }
}

//Q3-Q5
namespace puissance4
{
    //介绍 https://lululataupe.com/tout-age/686-puissance-4
    //定义一个 puissance4 的游戏
    //包括一个矩阵 代表当前的棋盘 
    //包括一个可以下棋的位置，用来计算下一步可以放在那里，
    //包括一个可以下棋的垂直的位置，用来计算下一步可以放在多高的位置，
    //一般来说，最后一颗棋子放在col的最顶端时，就会把这个col移除
    //棋盘上是 0 1 2 这三个记号，表示没有棋子，有红子，有黄子
    //展示方法：打印棋盘
    public enum Case { Void=0, Red=1, Yellow=2}

    public class PositionP4 : projet.Position
    {
        private int cols;
        private int lins;
        private Case[, ] table;


        private int currentX;
        private int currentY;

        private ArrayList possibleCoup;
        private ArrayList nextXPosition;

        private int hashCode = -1;

        private Resultat resultat = Resultat.indetermine;
        public override Resultat Eval
        {
            get
            {
                if (resultat == Resultat.indetermine)
                {
                    resultat = CheckWinning(currentX, currentY);
                    return resultat;
                }
                else
                {
                    return resultat;
                }
            }
        }

        //都是些constructor
        public PositionP4(bool j1aletrait) : base(j1aletrait)
        {
            Eval = Resultat.indetermine;
        }
        //public PositionP4(bool j1aletrait, Case[, ] table, int nbCoups, ArrayList possibleCoup, ArrayList nextXPosition) : this(j1aletrait) //调用上面的PositionA(bool j1aletrait)
        //{
        //    this.table = table;
        //    this.possibleCoup = possibleCoup;
        //    this.nextXPosition = nextXPosition;
        //    this.NbCoups = nbCoups;
        //}
        public PositionP4(bool j1aletrait, int lins, int cols) : this(j1aletrait) //调用上面的PositionA(bool j1aletrait)
        {
            this.NbCoups = cols; //总共可以行动的次数，起始是 1 2 3  4 5 6
            this.lins = lins;
            this.cols = cols;
            this.table = new Case[lins, cols];
            InitTable(this.table, lins, cols);
            this.possibleCoup = NewPossibleCoup(cols);
            this.nextXPosition = NewnextXPosition(lins, cols);
        }
        public PositionP4(bool j1aletrait, int lins, int cols, int currentX, int currentY, int NbCoups) : this(j1aletrait) //调用上面的PositionA(bool j1aletrait)
        {
            this.NbCoups = NbCoups; //总共可以行动的次数，起始是 1 2 3 4 5 6
            this.lins = lins;
            this.cols = cols; 
            this.currentX = currentX;
            this.currentY = currentY;
            this.table = new Case[lins, cols];
            this.possibleCoup = new ArrayList();
            this.nextXPosition = new ArrayList();
        }

        //新的棋盘
        private void InitTable(Case[, ] table, int lins, int cols)
        { 
            for (int i = 0; i < lins; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    table[i, j] = Case.Void;
                }
            }
        }
        //新的可下棋的位置
        private ArrayList NewPossibleCoup(int cols)
        {
            ArrayList possibleCoup = new ArrayList();
            for (int i = 0; i < cols; i++) { possibleCoup.Add(i);}
            return possibleCoup;
        }
        //新的可下棋的高度
        private ArrayList NewnextXPosition(int lins, int cols)
        {
            ArrayList nextXPosition = new ArrayList();
            for (int i = 0; i < cols; i++) { nextXPosition.Add(lins - 1); }
            return nextXPosition;
        }

        //打印当前的游戏状态
        public override void Affiche()
        {
            Console.WriteLine("\n========================\n" +
                "Joueur {0} a le trait ",
                this.j1aletrait ? 1 : 0 );

            for (int i = 0; i < lins; i++)
            {
                for (int j = 0; j < cols; j++)
                { 
                    System.Console.Write("{0} ", table[i,j]==Case.Void ? "_" : table[i, j] == Case.Red ? "X" : "O");
                }
                System.Console.WriteLine();
            }

            System.Console.Write("possibleCoup:[");
            foreach (int i in possibleCoup)
            {
                System.Console.Write("{0} ", i);
            }
            System.Console.WriteLine("]");

            System.Console.Write("nextXPosition:[");
            foreach (int i in nextXPosition)
            {
                System.Console.Write("{0} ", i);
            }
            System.Console.WriteLine("]");
        }

        //用来复制一个当前的游戏状态
        public override Position Clone()
        {
            PositionP4 newposition = new PositionP4(this.j1aletrait, lins, cols, currentX, currentY, NbCoups);
            newposition.possibleCoup = (ArrayList)possibleCoup.Clone();
            newposition.nextXPosition = (ArrayList)nextXPosition.Clone();
            newposition.table = table.Clone() as Case[,];
            return newposition;
        }
        //对比两个position
        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(this.GetType()))
            {
                return false;
            }
            if (obj is null)
            {
                return false;
            }
            //return false;
            return obj.GetHashCode() == this.GetHashCode();

        }
        //拿到position 的 hashcode
        public override int GetHashCode()
        {
            if (this.hashCode == -1)
            {
                StringBuilder myStringBuilder = new StringBuilder("Hello World!");
                foreach (Case t in table)
                {
                    myStringBuilder.Append(t.ToString("D"));
                }
                this.hashCode = myStringBuilder.ToString().GetHashCode();
            }

            return this.hashCode;
        }

        //做出动作，完成一次游戏，也就放下一颗棋子,  i \in [0,1,2,3,4,5,6]
        //让棋子自然落在一个col的最下面，如果col是满的，就重新选择，如果col的位置超出了范围，就重新选择
        //如果数字不合理 就报错
        //如果数字正常，就放下一颗棋子
        //然后轮到对方下棋
        public override void EffectuerCoup(int i)
        {

            if (i >= this.possibleCoup.Count)
            {
                Console.WriteLine("Illegal action, the colomn is out of table.");
                return;
            }

            if (i >= NbCoups)
            {
                Console.WriteLine("Illegal action, out of range.");
                return;
            }
            //拿到下棋的格子位置
            currentY = (int)possibleCoup[i];
            currentX = (int)nextXPosition[currentY];
            if (currentX < 0)
            {
                Console.WriteLine("Illegal action, this colomn is full.");
                return;
            }
            //下棋
            if (this.j1aletrait)
            {
                table[currentX, currentY] = Case.Yellow;
            }
            else
            {
                table[currentX, currentY] = Case.Red;
            }
            //判断一下是否有人胜利
            Resultat res = Eval;
            if (res == Resultat.j0gagne || res == Resultat.j1gagne)
            {
                NbCoups = 0;
                return;
            }
            //更新可以下棋的位置
            nextXPosition[currentY] = (int)nextXPosition[currentY] - 1;
            if ((int)nextXPosition[currentY] < 0)
            {
                possibleCoup.Remove(currentY);
                //更新coup的数量
                NbCoups -= 1;
            }


            //更新j1 a le trait; 轮到对方下棋
            this.j1aletrait = !this.j1aletrait;
            
        }
        public Resultat CheckWinning(int x, int y)
        {
            int sum = 0;
            int tmpX = 0;
            int tmpY = 0;
            Case toCheck;
            Resultat toWin;
            if (this.j1aletrait)
            {
                toCheck = Case.Yellow;
                toWin = Resultat.j1gagne;
            }
            else
            {
                toCheck = Case.Red;
                toWin = Resultat.j0gagne;
            }

            for(int j = 0; j < 4; j++) { 
                //横着
                sum = 0;
                for (int i = 0; i < 4; i++)
                { 
                    tmpX = (i+4)%4 + x - j;
                    if (tmpX < 0 || tmpX >= lins) continue;
                    tmpY = y;
                    if (table[tmpX, tmpY] == toCheck) {
                        sum += 1;
                    }
                }
                if (sum == 4)
                    return toWin;

                //斜着 1
                sum = 0;
                for (int i = 0; i < 4; i++)
                {
                    tmpX = (i + 4) % 4 + x - j;
                    if (tmpX < 0 || tmpX >= lins) continue;
                    tmpY = (i + 4) % 4 + y - j;
                    if (tmpY < 0 || tmpY >= cols) continue;
                    if (table[tmpX, tmpY] == toCheck)
                    {
                        sum += 1;
                    }
                }
                if (sum == 4)
                    return toWin;

                //斜着 2
                sum = 0;
                for (int i = 0; i < 4; i++)
                {
                    tmpX = (i + 4) % 4 + x - j;
                    if (tmpX < 0 || tmpX >= lins) continue;
                    tmpY = ( -i + 4) % 4 + y + j;
                    if (tmpY < 0 || tmpY >= cols) continue;
                    if (table[tmpX, tmpY] == toCheck)
                    {
                        sum += 1;
                    }
                }
                if (sum == 4)
                    return toWin;

                //竖着
                sum = 0;
                for (int i = 0; i < 4; i++)
                {
                    tmpX = x;
                    tmpY = (i + 4) % 4 + y - j;
                    if (tmpY < 0 || tmpY >= cols) continue;
                    if (table[tmpX, tmpY] == toCheck)
                    {
                        sum += 1;
                    }
                }
                if (sum == 4)
                    return toWin; 
            }
            //平局 没有可以下棋的位置了
            if (possibleCoup.Count == 0)
            {
                return Resultat.partieNulle;
            }
            return Resultat.indetermine;
        }
    }

    public class JoueurHumainP4 : projet.Joueur
    {
        public override int Jouer(Position p)
        {
            //询问玩家输入 最后把玩家的选择发回去
            //p.Affiche();
            Console.WriteLine("Choose your action from range [0, {0}[ for possible colomns ", p.NbCoups);
            int intTemp = Convert.ToInt32(Console.ReadLine());
            return intTemp;
        }
    }
    public class TestQ5
    {
        public TestQ5()
        {
            int cols = 7;
            int lins = 6;
            PositionP4 pInitiale = new PositionP4(true, lins, cols);
            JoueurHumainP4 j0 = new JoueurHumainP4();
            JMCTS j1 = new JMCTS(90, 100, 100);

            Partie partie = new Partie(j1, j0, pInitiale);
            partie.Commencer(true);
            Console.ReadLine();
        }
    }
}
//Q6
namespace jmctsq6
{
    public class JMCTSQ6 : JMCTS
    {
        protected static Hashtable nouedMap = new Hashtable();

        public JMCTSQ6(float a, float b, int temps) : base(a, b, temps)
        {
        }
        public override int Jouer(Position p)
        {
            sw.Restart();
            Func<int, int, float> phi = (W, C) => (a + W) / (b + C);

            if (nouedMap.ContainsKey(p.GetHashCode()))
            {
                racine = (Noeud)nouedMap[p.GetHashCode()];
            }
            else
            {
                racine = new Noeud(null, p);
            }

            int iter = 0;
            while (sw.ElapsedMilliseconds < temps)
            {
                Noeud no = racine;

                do // Sélection
                {
                    no.CalculMeilleurFils(phi);
                    no = no.MeilleurFils();

                } while (no.cross > 0 && no.fils.Length > 0);


                int re = JeuHasard(no.p); // Simulation

                while (no != null) // Rétropropagation
                {
                    no.cross += 2;
                    no.win += re;
                    no = no.pere;
                }
                iter++;
            }
            racine.CalculMeilleurFils(phi);

            //Console.WriteLine("{0} itérations", iter);
            //Console.WriteLine(racine);
            //把本次算过得子节点都放进map去
            for (int i = 0; i < racine.fils.Length; i++)
            {
                if (racine.fils[i] != null)
                {
                    nouedMap.Add(racine.fils[i].p.GetHashCode(), racine.fils[i]);
                }
            }

            return racine.indiceMeilleurFils;

        }
        public override void NouvellePartie()
        {
            nouedMap = new Hashtable();
        }
    }
    public class TestQ6
    {
        public TestQ6()
        {
            int cols = 7;
            int lins = 6;
            Position pInitiale = new puissance4.PositionP4(true, lins, cols);
            puissance4.JoueurHumainP4 j0 = new puissance4.JoueurHumainP4();
            JMCTS j1 = new JMCTSQ6(90, 100, 100);

            Partie partie = new Partie(j1, j0, pInitiale);
            partie.Commencer(true);
            Console.ReadLine();
            Console.ReadLine();

        }
    }
}

//Q7
namespace experience
{
    //对puissance4 做实验，
    //实验设计：
    //给定MCTS的运行时间 time=100 ms， 我们尝试变化 参数a 的大小，以找到最好的参数 a
    //游戏参与者有总共 PLAYER_SIZE = 10 人，每个人使用一个 参数a。a是在10 到10*10 的范围中。
    //a的大小可以通过调整step和SIZE 获得
    //每次游戏选择 参数a 大小不同的两个玩家 a1 a2，并且计算他们对对方的胜率。
    //实验结果是一个矩阵，里面放着的数据是 参数a1 a2 从小到大的变化过程中，MCTS 的胜率 ： 胜/负/平

    public class TestQ7
    {
        int mcts_time = 20;
        int PLAYER_SIZE = 10;
        //int[] rangeA = new int[] { 10, 100};
        int step = 10;
        int[] colomnA = new int[10];

        int EXP_SIZE = 40;
        public TestQ7()
        {
            for (int i = 1; i <= PLAYER_SIZE; i++) {
                colomnA[i-1] = step * i;
            }
            experienceA();
        } 


        public void experienceA()
        {
            int[,,] res = new int[PLAYER_SIZE, PLAYER_SIZE, 3];
            experienceA(res, colomnA, step, mcts_time, EXP_SIZE);
            for (int i = 0; i < PLAYER_SIZE; i += 1)
            {
                Console.Write("[");
                for (int j = 0; j < PLAYER_SIZE; j += 1)
                {
                    Console.Write("[");
                    for (int k = 0; k < 3; k += 1)
                    {
                        Console.Write("{0} ", res[i, j, k]);
                    }
                    Console.Write("],\t");
                }
                Console.Write("],\n");
            }
            Console.ReadLine();
        }
        public void experienceA(int[, , ] res, int [] colomnA, int step, int mcts_time, int exp_size) {
            for (int i= colomnA[0]; i<= colomnA[colomnA .Length- 1]; i += step)
            {
                for (int j = colomnA[0]; j <= colomnA[colomnA.Length - 1]; j += step)
                {
                    Console.WriteLine("Experience on a1={0} a2={1}", i,j);
                    experienceA(res, i, j, mcts_time, exp_size);
                }
            }
        }
        public void experienceA(int[,,] res, int a1, int a2, int mcts_time, int exp_size)
        {
            puissance4.PositionP4 pInitiale = new puissance4.PositionP4(true, 6, 7);
            //JMCTS j1 = new JMCTS(a1, a1, mcts_time);
            //JMCTS j0 = new JMCTS(a2, a2, mcts_time);
            JMCTS j1 = new jmctsq6.JMCTSQ6(a1, a1, mcts_time);
            JMCTS j0 = new jmctsq6.JMCTSQ6(a2, a2, mcts_time);
            for (int i = 0; i < exp_size; i++)
            {
                Partie partie = new Partie(j1, j0, pInitiale);
                partie.Commencer(false);
                
                if (partie.r == Resultat.j1gagne)
                {
                    res[a1 / step - 1, a2 / step - 1, 0] += 1;
                }
                else if (partie.r == Resultat.j0gagne)
                {
                    res[a1 / step - 1, a2 / step - 1, 1] += 1;
                }
                else if(partie.r == Resultat.partieNulle)
                {
                    res[a1 / step - 1, a2 / step - 1, 2] += 1;
                }
            }
        }
    }
}
/*实验结果
 * 结果分析：当 a1 a2 大小相当时，结果差不多，当 a1 << a2 时，胜率显著增加，
 * // 假设j1 在40回合比赛中赢（占优势）得1分 平得0.5分 输（占劣势）得0分 ，
 * // j1（先手，lins）的最优选择是 a=10 ，得10分
 * // j0（后手，cols）的最优选择也是 a=10 ，得9分
[[20 16 4 ],    [27 9 4 ],      [29 8 3 ],      [37 3 0 ],      [34 4 2 ],      [34 4 2 ],      [36 2 2 ],      [35 3 2 ],      [39 1 0 ],      [36 2 2 ],      ],
[[17 18 5 ],    [15 21 4 ],     [24 16 0 ],     [25 14 1 ],     [31 7 2 ],      [34 5 1 ],      [34 4 2 ],      [34 4 2 ],      [32 6 2 ],      [37 2 1 ],      ],
[[10 30 0 ],    [17 20 3 ],     [25 15 0 ],     [23 17 0 ],     [26 12 2 ],     [28 10 2 ],     [31 9 0 ],      [34 6 0 ],      [30 9 1 ],      [33 6 1 ],      ],
[[13 22 5 ],    [20 20 0 ],     [18 20 2 ],     [26 12 2 ],     [22 17 1 ],     [26 14 0 ],     [28 11 1 ],     [30 10 0 ],     [31 7 2 ],      [33 5 2 ],      ],
[[11 27 2 ],    [16 23 1 ],     [15 24 1 ],     [18 21 1 ],     [28 11 1 ],     [25 13 2 ],     [26 13 1 ],     [30 9 1 ],      [30 7 3 ],      [29 10 1 ],     ],
[[8 31 1 ],     [11 28 1 ],     [15 23 2 ],     [19 19 2 ],     [20 20 0 ],     [19 18 3 ],     [26 14 0 ],     [23 15 2 ],     [29 10 1 ],     [23 15 2 ],     ],
[[2 36 2 ],     [12 28 0 ],     [10 28 2 ],     [18 19 3 ],     [20 20 0 ],     [24 14 2 ],     [20 19 1 ],     [23 16 1 ],     [28 10 2 ],     [28 12 0 ],     ],
[[3 37 0 ],     [4 33 3 ],      [10 30 0 ],     [15 24 1 ],     [17 20 3 ],     [18 21 1 ],     [19 20 1 ],     [21 15 4 ],     [19 18 3 ],     [22 17 1 ],     ],
[[7 32 1 ],     [6 33 1 ],      [8 30 2 ],      [11 27 2 ],     [18 21 1 ],     [19 21 0 ],     [19 18 3 ],     [25 13 2 ],     [20 19 1 ],     [22 16 2 ],     ],
[[3 37 0 ],     [5 35 0 ],      [9 29 2 ],      [9 31 0 ],      [17 21 2 ],     [15 23 2 ],     [19 19 2 ],     [13 27 0 ],     [22 17 1 ],     [25 14 1 ],     ],

 * // 修改后的结果，这个似乎时间稍稍短了一点点 但是结果区别不大
 [[22 14 4 ],    [21 14 5 ],     [25 12 3 ],     [36 4 0 ],      [29 8 3 ],      [28 12 0 ],     [35 5 0 ],      [33 6 1 ],      [34 3 3 ],      [38 2 0 ],      ],
[[17 19 4 ],    [21 15 4 ],     [20 17 3 ],     [29 9 2 ],      [31 8 1 ],      [26 11 3 ],     [33 6 1 ],      [33 5 2 ],      [35 3 2 ],      [33 4 3 ],      ],
[[17 18 5 ],    [16 21 3 ],     [23 16 1 ],     [22 17 1 ],     [26 10 4 ],     [25 14 1 ],     [33 7 0 ],      [25 11 4 ],     [32 6 2 ],      [31 8 1 ],      ],
[[9 26 5 ],     [16 24 0 ],     [16 21 3 ],     [17 20 3 ],     [25 13 2 ],     [22 17 1 ],     [27 12 1 ],     [28 8 4 ],      [27 12 1 ],     [25 15 0 ],     ],
[[10 28 2 ],    [15 24 1 ],     [11 24 5 ],     [19 18 3 ],     [22 14 4 ],     [19 19 2 ],     [25 13 2 ],     [24 15 1 ],     [31 9 0 ],      [32 7 1 ],      ],
[[7 32 1 ],     [8 30 2 ],      [18 21 1 ],     [19 19 2 ],     [23 16 1 ],     [23 15 2 ],     [22 18 0 ],     [17 21 2 ],     [28 11 1 ],     [24 16 0 ],     ],
[[5 33 2 ],     [9 27 4 ],      [11 27 2 ],     [16 23 1 ],     [16 22 2 ],     [20 20 0 ],     [25 14 1 ],     [24 14 2 ],     [30 9 1 ],      [27 12 1 ],     ],
[[2 37 1 ],     [7 31 2 ],      [13 23 4 ],     [12 27 1 ],     [15 19 6 ],     [20 18 2 ],     [22 17 1 ],     [20 19 1 ],     [23 15 2 ],     [24 16 0 ],     ],
[[6 32 2 ],     [9 29 2 ],      [15 25 0 ],     [16 23 1 ],     [18 20 2 ],     [14 23 3 ],     [20 18 2 ],     [17 22 1 ],     [22 18 0 ],     [28 12 0 ],     ],
[[4 36 0 ],     [8 32 0 ],      [12 28 0 ],     [15 24 1 ],     [19 20 1 ],     [16 22 2 ],     [13 25 2 ],     [18 22 0 ],     [20 18 2 ],     [20 17 3 ],     ],
     */


namespace jmctsp
{
    public static class StaticRandom
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static int Rand()
        {
            return random.Value.Next();
        }
    }

    public class JMCTSp : jmctsq6.JMCTSQ6
    {
        int N = 4;
        public JMCTSp(float a, float b, int temps) : base(a, b, temps)
        {

        }

        public override int JeuHasard(Position p)
        {
            Position q = p.Clone();
            int re = 1;
            while (q.NbCoups > 0)
            {
                //q.EffectuerCoup(gen.Next(0, q.NbCoups));
                q.EffectuerCoup(StaticRandom.Rand() % q.NbCoups);
            }
            if (q.Eval == Resultat.j1gagne) { re = 2; }
            if (q.Eval == Resultat.j0gagne) { re = 0; }
            return re;
        }
        public override int Jouer(Position p)
        {
            sw.Restart();
            Func<int, int, float> phi = (W, C) => (a + W) / (b + C);

            if (nouedMap.ContainsKey(p.GetHashCode()))
            {
                racine = (Noeud)nouedMap[p.GetHashCode()];
            }
            else
            {
                racine = new Noeud(null, p);
            }

            int iter = 0;
            while (sw.ElapsedMilliseconds < temps)
            {
                Noeud no = racine;

                do // Sélection
                {
                    no.CalculMeilleurFils(phi);
                    no = no.MeilleurFils();

                } while (no.cross > 0 && no.fils.Length > 0);

                Task<int>[] t = new Task<int>[N];
                for (int i = 0; i < N; i++)
                {
                    int j = i;
                    t[i] = Task.Run(() => JeuHasard(no.p));
                }
                Task.WaitAll(t);

                int re = 0;
                for (int i = N - 1; i >= 0; i--)
                {
                    re += t[i].Result;
                }

                while (no != null) // Rétropropagation
                {
                    no.cross += 2;
                    no.win += re;
                    no = no.pere;
                }
                iter++;
            }
            racine.CalculMeilleurFils(phi);

            //Console.WriteLine("{0} itérations", iter);
            //Console.WriteLine(racine);
            //把本次算过得子节点都放进map去
            for (int i = 0; i < racine.fils.Length; i++)
            {
                if (racine.fils[i] != null)
                {
                    nouedMap.Add(racine.fils[i].p.GetHashCode(), racine.fils[i]);
                }
            }

            return racine.indiceMeilleurFils;

        }

    }


    public class TestQ9
    {
        int mcts_time = 100;
        int a = 10;

        int EXP_SIZE = 30;
        int[] res = new int[3];
        public TestQ9()
        {
            experienceQ9();
            Console.ReadLine();
        }


        public void experienceQ9()
        {
            puissance4.PositionP4 pInitiale = new puissance4.PositionP4(true, 6, 7);
            //JMCTS j1 = new JMCTSQ6(a1, a1, mcts_time);
            //JMCTS j0 = new JMCTSm(a2, a2, mcts_time);
            //JMCTS j1 = new JMCTS(a, a, mcts_time);
            //JMCTS j0 = new jmctsp.JMCTSp(a, a, mcts_time);
            //JMCTS j0 = new jmctsq6.JMCTSQ6(a, a, mcts_time);
            JMCTS j1 = new JMCTS(a, a, mcts_time);
            JMCTS j0 = new jmctsp.JMCTSp(a, a, mcts_time);
            for (int i = 0; i < EXP_SIZE; i++)
            {
                Partie partie = new Partie(j1, j0, pInitiale);
                partie.Commencer(false);

                if (partie.r == Resultat.j1gagne)
                {
                    res[0] += 1;
                }
                else if (partie.r == Resultat.j0gagne)
                {
                    res[1] += 1;
                }
                else if (partie.r == Resultat.partieNulle)
                {
                    res[2] += 1;
                }
            }
            Console.Write("[");
            for (int k = 0; k < 3; k += 1)
            {
                Console.Write("{0} ", res[k]);
            }
            Console.Write("]\n");
        }
    }
}

 namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
                /*
                Console.WriteLine("TestQ2");
                allumettes.TestQ2 testQ2 = new allumettes.TestQ2();
                */

                /*
                Console.WriteLine("TestQ5");
                puissance4.TestQ5 testQ5 = new puissance4.TestQ5();
                */
                /*
                Console.WriteLine("TestQ6");
                jmctsq6.TestQ6 testQ6 = new jmctsq6.TestQ6();
                */

                /*
                Console.WriteLine("TestQ7");
                experience.TestQ7 testQ7 = new experience.TestQ7();
                */ 
                Console.WriteLine("TestQ9");
                jmctsp.TestQ9 TestQ9 = new jmctsp.TestQ9();

        }
    }
}



