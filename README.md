# Dynamic For Unity

### 简介

Dynamic For Unity，为Unity实现了dynamic能力

主要有如下两个功能

### 动态字段 - DynamicField 【核心】

在Unity中，实现类似【dynamic关键字】的功能
    
DynamicField，可以储存"任意"类型，不论是class还是struct，且类型转换0GC，无装/拆箱，无反射

注："任意"，指具有一定的限制，问题不大，后面会讲

### 动态对象 - DynamicObject

在Unity中，实现类似【jsObject (就是js中的object)】动态字段的功能

DynamicObject，可以拥有任意数量，"任意"类型的字段（基于上面的DynamicField实现）

### 安装

Unity最小版本 `2020.3.17f1`（更小的版本如3.12f1可能也可以，未做测试）

1. 通过 [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) 安装 `System.Runtime.CompilerServices.Unsafe` 包
2. 通过 `OpenUPM` 安装 https://openupm.com/packages/com.gm.dynamic/
