# -*- coding: utf-8 -*-
"""
Created on Wed Feb  3 18:13:00 2021

@author: Administrater
"""


import tushare as ts
import pandas as pd
import datetime

token='6ff3a72c33b76fbe151e5790aa969568e1b34f380f5cc9ce44dbe2ee'
ts.set_token(token)


pro=ts.pro_api()
pc=pro.daily_basic(exchange='', list_status='L', fields='ts_code,symbol,name,area,industry,list_date')

pa=pro.daily(ts_code='000001.sz', start_date='20190101', end_date='20200213')
pb=pro.get_stock_basics('2020-06-22')

