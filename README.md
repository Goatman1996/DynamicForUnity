# Dynamic For Unity

### 简介

Dynamic For Unity，为Unity实现了dynamic能力

主要有如下两个功能

### 动态字段 - DynamicField 【核心】

在Unity中，实现类似【dynamic关键字】的功能
    
DynamicField，可以储存"任意"类型，不论是class还是struct，且类型转换0GC，无装/拆箱，无反射

注：["任意"]，指具有一定的限制，问题不大，后面会讲

### 动态对象 - DynamicObject

在Unity中，实现类似【jsObject (就是js中的object)】动态字段的功能

DynamicObject，可以拥有任意数量，"任意"类型的字段（基于上面的DynamicField实现）

### 安装

Unity最小版本 `2022.3.17f1`（更小的版本如3.12f1可能也可以，未做测试）

第1步. 通过 [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) 安装 `System.Runtime.CompilerServices.Unsafe 6.0.0` 包

第2步. 通过 `OpenUPM` 安装 https://openupm.com/packages/com.gm.dynamic/ 本插件

### 使用

动态字段 - DynamicField 【核心】
---

以下功能不包含任何GC，且无装箱拆箱，无反射

DynamicField 可以存储"任意"类型

API介绍

``` csharp
using GM.Dynamic;

// DynamicField is struct
DynamicField dynamicField;

// -------------
// As 是 可读可写的
// -------------
dynamicField.As<T>() = (T)value;
T value = dynamicField.As<T>();
// 特别的 一旦As，若当前DynamicField的值类型不匹配，DynamicField的值会重置为default(T)



// -------------
// Is 类型判断
// -------------
bool is_T = dynamicField.Is<T>();
// 对于值类型的T，直接判断是否是同类型
// 对于引用类型，有两种情况
// 1. value != null 时, return value is T
// 2. value == null 时, return false



// -------------
// TryAs 是 只读的
// -------------
dynamicField.As<T1>() = value_T1;
// 这会得到 default(T2)，且不改变原有值，值依旧是 value_T1
T2 value = dynamicField.TryAs<T2>();
```

以下是详细是示例

``` csharp
using GM.Dynamic;

// DynamicField is struct
private DynamicField dynamicField;

public int t_Int;
public float t_Float;
public string t_String;
public bool t_Bool;
public Vector3 t_Vector3;
public GameObject t_GameObject;
public Transform t_Transform;
public UnityEngine.Object t_UnityObject;

// -------------DynamicField.As<T>() 示例
private void As_Sample()
{
  // DynamicField.As<T>() 方法是可读可写的

  dynamicField.As<int>() = Time.frameCount;
  this.t_Int = dynamicField.As<int>();

  dynamicField.As<float>() = Time.deltaTime;
  this.t_Float = dynamicField.As<float>();

  dynamicField.As<string>() = "Hello";
  this.t_String = dynamicField.As<string>();

  dynamicField.As<Vector3>() = Vector3.one;
  dynamicField.As<Vector3>().x = 2;
  this.t_Vector3 = dynamicField.As<Vector3>();

  // As<T> T 为class对象时，可自动进行多态判断
  dynamicField.As<GameObject>() = this.gameObject;
  this.t_UnityObject = dynamicField.As<UnityEngine.Object>();
  this.t_GameObject = dynamicField.As<GameObject>();

  dynamicField.As<UnityEngine.Object>() = this.transform;
  this.t_UnityObject = dynamicField.As<UnityEngine.Object>();
  this.t_Transform = dynamicField.As<Transform>();

   // 特别情况 ------ 当类型不匹配时
   // 当T和已经保存的值的类型不一致时，会将DynamicField的值重置为default(T)
   dynamicField.As<bool?>() = true;
   // 类型不匹配 bool? != bool 
   // 导致 DynamicField的值重置为default(T)
   // 所以下面的结果是 this.t_Bool == false
   this.t_Bool = dynamicField.As<bool>();

   dynamicField.As<UnityEngine.Object>() = this.transform;
   // 类型不匹配 Transform != GameObject
   // 导致 DynamicField的值重置为default(T)
   // 所以下面两个都是null
   this.t_GameObject = dynamicField.As<GameObject>();
   this.t_UnityObject = dynamicField.As<UnityEngine.Object>();
}

// -------------DynamicField.TryAs<T>() 示例
private void TryAs_Sample()
{
  // DynamicField.TryAs<T>() 方法是只读的
  // 所以不可以 dynamicField.TryAs<int>() = 10;

  dynamicField.As<int>() = 10;
  // 类型不匹配时，返回default(T)，且不修改已保存的值
  // 下面的 this.t_Float == 0
  this.t_Float = dynamicField.TryAs<float>();
  // this.t_Int 仍然等于 10
  this.t_Int = dynamicField.TryAs<int>();

  dynamicField.As<UnityEngine.Object>() = this.transform;
  // 类型不匹配时，返回default(T)，且不修改已保存的值
  // 下面的 this.t_GameObject == null
  this.t_GameObject = dynamicField.TryAs<GameObject>();
  // this.t_Transform == this.transform
  this.t_Transform = dynamicField.TryAs<Transform>();
}

// -------------DynamicField.Is<T>() 示例
private void Is_Sample()
{
  // DynamicField.Is<T>() 类型判断

  // 值类型---------
  dynamicField.As<int>() = 10;
  Debug.Log(dynamicField.Is<int>());// true
  Debug.Log(dynamicField.Is<float>());// false    

  // 引用类型---------
  dynamicField.As<Transform>() = this.transform;
  Debug.Log(dynamicField.Is<UnityEngine.Object>());// true
  Debug.Log(dynamicField.Is<GameObject>());// false

  // 引用类型---------
  dynamicField.As<UnityEngine.Object>() = this.gameObject;
  Debug.Log(dynamicField.Is<UnityEngine.Object>());// true
  Debug.Log(dynamicField.Is<GameObject>());// true
  Debug.Log(dynamicField.Is<Transform>());// false


  dynamicField.As<UnityEngine.Object>() = null;
  // 引用类型，且值为空时，Is均返回false
  Debug.Log(dynamicField.Is<UnityEngine.Object>());// false
  Debug.Log(dynamicField.Is<GameObject>());// false
  Debug.Log(dynamicField.Is<Transform>());// false
  Debug.Log(dynamicField.Is<Vector3>());// false
}
```

### "任意"
---
重要！！！"任意"类型 指 任何sizeof(T) <= 56 的值类型 和 任意引用类型（sizeof(引用类型) == 地址长度）
---

说明：为什么是56呢，因为虽然是“动态”字段，但是总得存值，总得确认内存分配大小

当尝试使用 sizeof(T) > 56 时。会抛出 StructOverSizeException 异常

TODO:后续可能会考虑一定程度上开放MaximumSize的设置

动态对象 - DynamicObject 的使用方法
---

动态对象，是基于DynamicField实现的，模仿了jsObject动态字段的功能

API介绍
``` csharp
using GM.Dynamic;

// DynamicObject is class
DynamicObject dynamicObject = new();

// 动态对象，本质上是一个"字典"，string为key，DynamicField为value
// DynamicObject 的基本API与 DynamicField 一致：
// ------- As<T>()      等价于 As<T>(typeof(T).Name)
// ------- TryAs<T>()   等价于 TryAs<T>(typeof(T).Name)
// ------- Is<T>()      等价于 Is<T>(typeof(T).Name)
// 事实上 typeof(T).Name 有GC，所以实际代码做了缓存
dynamicObject.As<int>() = 1；
int int_1 = dynamicObject.TryAs<int>();
bool is_Int = dynamicObject.Is<int>();
// 上下相等
dynamicObject.As<int>(typeof(int).Name) = 1；
int int_1 = dynamicObject.TryAs<int>(typeof(int).Name);
bool is_Int = dynamicObject.Is<int>(typeof(int).Name);

// ----------
// DynamicObject.Reset
// 清空一个字段，可以腾出空间
// ----------
dynamicObject.Reset("Field Key");
dynamicObject.Reset<T>();// dynamicObject.Reset(typeof(T).Name);

// ----------
// DynamicObject.Every<T>()
// 可遍历所有T类型的字段
// ----------
DynamicEnumerable<T> every_T_Enumerable = dynamicObject.Every<T>();
DynamicEnumerator<T> every_T_Enumerator = every_T_Enumerable.GetEnumerator();
// 可foreach 遍历 （只读遍历）
foreach (T item in every_T_Enumerable){ }
// 可手动遍历 （可写遍历）
while (every_T_Enumerator.MoveNext())
{
  // RefCurrent 可写（假设T = int）
  every_T_Enumerator.RefCurrent += 1;
  // Current 只读（假设T = int）
  int i = every_T_Enumerator.Current;
}

```

以下是详细是示例

``` csharp
using GM.Dynamic;

// DynamicObject is class
DynamicObject dynamicObject = new();

// DynamicObject 基本 示例
private void Object_Sample()
{
  dynamicObject.As<int>("Int_1") = 4;
  int int_1 = dynamicObject.As<int>("Int_1");

  dynamicObject.As<float>("Float_1") = 5f;
  float float_1 = dynamicObject.As<float>("Float_1");

  dynamicObject.As<bool>("Bool_1") = true;
  // 这不会修改已经存在“Bool_1”的值
  float not_Float = dynamicObject.TryAs<float>("Bool_1");
  bool bool_1 = dynamicObject.TryAs<bool>("Bool_1");

  // console = 4_5_0_True
  Debug.Log($"{int_1}_{float_1}_{not_Float}_{bool_1}");

  dynamicObject.As<GameObject>("BattleTarget") = new GameObject();
  if (dynamicObject.TryAs<GameObject>("BattleTarget") != null)
  {
      // Fight
  }



  // 下面等于 dynamicObject.As<GameObject>(typeof(GameObject).Name) = this.gameObject 
  dynamicObject.As<GameObject>() = this.gameObject;
  // 下面等于 dynamicObject.As<Transform>(typeof(Transform).Name) = this.transform 
  dynamicObject.As<Transform>() = this.transform;

  // 不指定Key的写法，可以让一个DynamicObject使用起来好像是任何东西的多态一样
  if (dynamicObject.Is<GameObject>())
  {
      dynamicObject.TryAs<GameObject>().SetActive(false);
  }
  if (dynamicObject.Is<Transform>())
  {
      dynamicObject.TryAs<Transform>().position = Vector3.zero;
  }
  dynamicObject.TryAs<Rigidbody>()?.Move(Vector3.zero, Quaternion.identity);
}

// DynamicObject 特别功能 示例
private void Object_Reset_Sample()
{
  // 清空一个字段，可以腾出空间
  dynamicObject.Reset("Some Field");
  // 等价于 dynamicObject.Reset("GameObject");
  dynamicObject.Reset<GameObject>();

  dynamicObject.As<GameObject>() = this.gameObject;
  dynamicObject.As<Transform>() = this.transform;
  dynamicObject.As<int>("Int_1") = 1;
  dynamicObject.As<int>("Int_2") = 2;
  dynamicObject.As<int>("Int_3") = 3;

  // 只读 遍历
  foreach (var unityObject in dynamicObject.Every<UnityEngine.Object>())
  {
      // 会打印 this.gameObject
      // 会打印 this.transform
      Debug.Log(unityObject);
  }

  // 可写 遍历
  var everyInt = dynamicObject.Every<int>().GetEnumerator();
  while (everyInt.MoveNext())
  {
      everyInt.RefCurrent += 10;
  }

  foreach (var intValue in dynamicObject.Every<int>())
  {
      // 会打印 11 12 13
      Debug.Log(intValue);
  }
}
```

### 最后

觉得有趣的点个Star~

谢谢~

