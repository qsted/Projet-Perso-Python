# -*- coding: utf-8 -*-
"""
Created on Sun Nov 15 21:11:03 2020

@author: Administrater
"""

'''
 
'''
print('A')

def main():
    print('B')
    
if __name__=='__main__':
    main()
    
print('C')

arr = [0,3,1,2,4]

def is_foo(parm):
    if parm=='foo':
        return True
    else:
        return False
print(is_foo('hi'))

def factorial(n):
    if n<=1:
        return 1 
    else: 
        return n*factorial(n-1)
    
print(factorial(5))   

def find_largest(number):
    maxvalue=number[0]
    for i in range(len(number)):
        if number[i]>maxvalue:
            maxvalue=number[i]
    return maxvalue
        

number=[1,2,3,100,89]

print(find_largest(number))

def average(table):
    if len(table)==0:
        return 0
    else:
        return sum(table)/len(table)


table=[1,2,3,4,5]

print(average(table))


def is_bool(i,j):
    if i==1 or j==1 or i+j ==1:
        return True 
    else: 
        return False

print(is_bool(1, 2))

def closest_to_zero(ints):
    if ints:
        index=0
        for i in range(len(ints)):
            if abs(ints[index])>= abs(ints[i]):            
                index=i
            if ints[index]==ints[i] and ints[index]<ints[i]:
                index=i
        return ints[index]
        
    else:
        return 0

ints=[-2,2,-1,1,3]

print(closest_to_zero(ints))


'''
in python 3 which can be used to create an exception 
'''
raise Exception()

raise Exception('exception')



'''
how to check bob in phone book

'bob' in phonebook
if this is dict bob in phonebook bob is key 

() exsist a[0]

list(dic.values())
list(dic.keys())
'''


'''
get last element of list 

arr(len(arr)-1)

arr[-1]

list(reversed(arr))[0]
'''

'''
sort a list 

arr.sort()
arr=sorted(arr)
'''

strs=['1','2','3']

list(map(int,strs))
[int(x) for x in strs]


def is_on_even_position(table,value):
    if value not in table:
        return False
    else: 
        index=table.index(value)
        if index%2==0:
            return True 
        else:
            return False

a=[1,2,3]

print(is_on_even_position(a, 3))



def pi_approx(pts):
    count=0
    for x,y in pts:
        if (x**2+y**2)**0.5<1:
            count+=1
    return 4.0*count/len(pts)

import random

rands=[]
for i in range(100000):
    arr=[random.random(),random.random()]
    rands.append(arr)

print(pi_approx(rands))


def reshape(n, str):
    str=str.replace(' ','')
    newstr=''
    for i in range(len(str)):
        if i != 0 and i % n==0:
            newstr += '\n'+str[i]
        else:
            newstr += str[i]

    return newstr

print(reshape(2, 'as cdef ghff'))


def nextWeek(d):
    import datetime
    for i in range(1,7):
        print(d+datetime.timedelta(days=i))

import datetime

d=datetime.date.today()

print(nextWeek(d))

def is_on_even_position(table,value):
    index=table.index(value)
    if index%2==0:
        return True 
    else:
        return False

table=[1,2,3]

print(is_on_even_position(table, 2))

def facto(n):
    if n<1:
        return 1
    else:
        return n*facto(n-1)
    
print(facto(4))


def fib(n):
    if n<1:
        return 1
    else: 
        return fib(n-1)+fib(n-2)
    

print(fib(5))


def find(number):
    maxnum=number[0]
    for i in number:
        if i>maxnum:
            maxnum=i
    return maxnum

number=[1,2,3,5,4]

print(find(number))


def average(table):
    if len(table)==0:
        return 0
    else:
        return sum(table)/len(table)
    
    
print(average(number))



def is_bool(i,j):
    if i==1 or j==1 or i+j ==1:
        return True
    else:
        return False


def is_twin(a,b):
    a=a.lower()
    b=b.lower()
    a=sorted(list(a))
    b=sorted(list(b))
    return a==b

a='qsted'
b='sqted'

print(is_twin(a, b))



def is_on_even_position(table,value):
    if value not in table:
        return False
    else: 
        index=table.index(value)
        if index%2==0:
            return True 
        else:
            return False

a=[1,2,3]

print(is_on_even_position(a, 3))


def pi_approx(pts):
    count=0
    for x,y in pts:
        if (x**2+y**2)**0.5<1:
            count+=1
    return 4.0*count/len(pts)

import random

rands=[]
for i in range(10):
    arr=[random.random(),random.random()]
    rands.append(arr)

print(pi_approx(rands))


def reshape(n, str):
    str=str.replace(' ','')
    newstr=''
    for i in range(len(str)):
        if i != 0 and i % n==0:
            newstr += '\n'+str[i]
        else:
            newstr += str[i]

    return newstr

print(reshape(2, 'as cdef ghff'))


import datetime

d=datetime.date.today()

print(d+datetime.timedelta(days=1))


def nextWeek(d):
    import datetime
    for i in range(1,7):
        print(d+datetime.timedelta(days=i))

import datetime

d=datetime.date.today()

print(nextWeek(d))



list1=[(x,y) for x in range(10) for y in range(10) if x%2==0 if y % 2!=0 ]

list1.sort()
list1.reverse()

sorted(list1)

x,y,z=1,2,3

tuple1 = (x**2 for x in range(10))#生成器

str1 = '<a href="http://www.fishc.com/dvd" target="_blank">'
str1[20:-36]



str3 = ('待卿长发及腰，我必凯旋回朝。'
'昔日纵马任逍遥，俱是少年英豪。'
'东都霞色好，西湖烟波渺。'
'执枪血战八方，誓守山河多娇。'
'应有得胜归来日，与卿共度良宵。'
'盼携手终老，愿与子同袍。')
str1='abcdefghigh'
str1[::2]
#'acegih'
print('{0}{1:.2f}'.format('pi= ',3.1415))

'''
Just for completeness as nobody else has mentioned it. 
The third parameter to an array slice is a step. 
So reversing a string is as simple as:
'''
#some_string[::-1]
#Or selecting alternate characters would be:

"H-e-l-l-o- -W-o-r-l-d"[::2] # outputs "Hello World"


def next():
    print('wozai next li')
    pre()

def pre():
    print('wozai pre li')

next()

# 如果没有返回值，返回一个none

def myfun():
    
    return 'caocao',520,3.14,True# fanhui tuple 

def myfun2():
    return 'caocao' 520 3.14 True#没有，会报错
# def myfun3():
#     return ['caocao',520,3.14,True]#fanhui list

def fun(var):
    var=123
    print(var,end='')#两个var无关
#print(var(520)) ->123520
    
#谨慎使用全局变量
    
def palindrom(string):
    lit1=list(string)
    lit2=list(reversed(lit1))
    if lit1==lit2:
        return 'yes'
    else:
        return 'no'
print(palindrom('aba'))
    

def funout():
    def funin():
        print('this is fun in')
    return funin()

funout()#visit funin
    def funout():
    def funin():
        print('this is fun in')
    return funin
funout()()  #注意和上边区别
go=funout()
go()

def fun_A(x,y=3):
    return x*y

[lambda x,y=3 :  x*y ]
lambda x: x if x%2 else None

def is_odd(x)：:
    if x%2:
        return x
    else:
        return None
[i for i in range(100) if not(i%3)]

import easygui as g
g.msgbox('hi')
msg='guess a number'
guess=g.integerbox(msg,'game',lowerbound=1,upperbound=10) 
    
#传递函数
def bar():
    print('i am a bar')
def foo(func):
    func()
foo(bar)
    
def foo(fun):
    def warp():
        print('start')
        fun()
        print('end')
        print(fun.__name__)
    return warp

@foo
def bar():
    print('i am in bar')
bar()

g=lambda x ,y : x+y

g(3,4)


class Girl:
    x=90

tom=Girl()

tom.x
tom.y=9

tom.__dict__

class Person:
    def __init__(self,name):
        self.name=name
    def get_name(self):
        print(self.name)
    def breast(self,n):
        self.breast=n
    def color(self,color):
        print(self.name,'is',color)
    def how(self):
        print(self.name,'breast is',self.breast)

girl=Person('canglaoshi') 
girl.breast(90)
girl.color('white')
girl.how()       
        
        
import sqlite3
conn=sqlite3.connect("lite.db")      
        
dir(conn)   













