# -*- coding: utf-8 -*-
"""
Created on Tue Nov 24 17:33:12 2020

@author: Administrater
"""


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

print(d-datetime.timedelta(days=7))


def nextWeek(d):
    import datetime
    for i in range(1,7):
        print(d+datetime.timedelta(days=i))

import datetime

d=datetime.date.today()

print(nextWeek(d))

a=[1,2,3]
b=[2,3,4]
for i in b:
    if i not in a:
        a.append(i)
print(a)



def bulk(self):
    print("%s is yelling...." %self.name)

class Dog(object):
    def __init__(self,name):
        self.name = name

    def eat(self,food):
        print("%s is eating..."%self.name,food)


d = Dog("NiuHanYang")
choice = input(">>:").strip()

if hasattr(d,choice):
    getattr(d,choice)
    #delattr(d,choice)
    
else:
    setattr(d,choice,bulk) #d.talk = bulk
    func = getattr(d, choice)
    func(d)

import threading, multiprocessing

def loop():
    x = 0
    while True:
        x = x ^ 1

for i in range(multiprocessing.cpu_count()):
    t = threading.Thread(target=loop)
    t.start()







