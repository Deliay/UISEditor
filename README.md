# UISEditor
	今天就是要告诉一些，UISEditor的道理

## UISEditor 程序结构

--------------

### 目录结构
- Bridge	
	> 外部属性转换器Convertor，用于属性页的属性转换。
- Controller
	> 界面逻辑控制器（各个界面逻辑在.xaml.cs里）
- Data
	> UIS的lexical analyzer和parser，还有核心数据抽象
- Render
	> UIS渲染脚本逻辑
- View 
	> UISEditor各个界面
- ViewModel
	> UISEditor运行时数据

-----------------------

### 程序执行

打开UIS -> 解析 -> ``UISObjectTree`` (``UISList``)

``UISObjectTree`` -> 属性浏览器

``UISObject`` -> `CombineValue()` -> 文本编辑器

文本编辑器 通过行号和 ``UISObjectTree`` 和 属性浏览器 `TwoWay Bind`

遍历 ``UISObjectTree`` 查找可渲染对象 -> 根据类型初始化渲染实例 ( `UISRenderable` ) -> 渲染到`Canvas`


文本编辑器 -> 触发编辑事件 -> 更新属性浏览器对应属性

属性浏览器 -> 触发更新事件 ->  `UISRenderable.Refresh()` 刷新渲染视图

-----------------------

### UIS 解析

Tokenize，然后Top-Down Parser，带一点简单回溯。

`UISEditor.Data.Lexical`	Lexical Analyzer

`UISEditor.Data.Parser`		Parser

-----------------------

### UISEditor支持的元素列表

	UIS没有过多的语义定义，但在UISEditor里强制定义了一些数据和类型，并对属性进行绑定

| 类型 | 适用元素 | 备注 | 继承自 | 备注 |
| :-: | :-: | :-: | :-: | :-: |
|元素基类|
| `UISObject` | 无 | 任意元素继承的基类 | 无 | 无 |
| `UISList` | 列表 | 实现的所有列表的非泛型基类 | `UISObject` |无 |
| `UISValue` | 无 | 作为任意属性值的基类 | `UISObject` |无 |
| `UISProperty` | 成员 | 是某类可渲染元素的成员和属性 | `UISObject` |无 |
| `UISComment` | 注释 | 以`#`开头的出现在元素上方的注释 | `UISObject` |无 |
| `UISFunctionalElement` | 标识 | 使用`@`符号开头的内容 | `UISObject` |无 |
|列表基类|
| `UISList<T>` | 列表 | 实现的所有列表的泛型基类 | `UISList` |无 |
| `UISGroup` | 组 | 使用`+`号分组的内容组 | `UISList<UISObject>` |无 |
| `UISElement<T>` | 列表 | 是元素列表的基列表 | `UISList<T>` |无 |
|列表实现|
| `UISAnimation`| 动画列表 | 用于动画声明的相关属性和值的列表 | `UISList<UISAnimationProperty>` |  管理`UISAnimationProperty` |
| `UISAnimationElement` | 动画属性列表 | 用于以`:`开头的一组动画定义的属性列表 | `UISElement<UISAnimation>` | 管理`UISAnimation` |
| `UISPredefineElement` | 预定义元素列表 | 用于以`@`开头的预定义元素的属性列表 | `UISElement<UISProperty>` |无 |
| `UISCustomElement` | 自定义元素列表 | 用于以`_`开头的自定义元素的属性列表 | ` UISElement<UISProperty>` |无 |
|属性实现|
| `UISNull` | 值 | 表示一个空元素的值 | `UISValue` |无 |
| `UISLiteralValue` | 值 | 是一系列字面立即值的基类，例如像素，百分比和计算式等 | `UISValue` | 无 | 
| `UISVector` | 值 | 表示由两个立即值(`UISLiteralValue`)组成的值 | `UISValue` | 通常一些二元属性会用到，例如`pos=(x,y)`,`size=(x,y)` |
| `UISRelativeVector` | 值 | 表示某两个立即值是依赖某个元素进行计算的 | `UISVector`| 通常在做一些依赖属性时会引入该属性 |
| `UISAnimationProperty` | 动画 | 动画元素的参数属性 | `UISValue` |无 |
| `UISFileName` | 文件名 | 表示一个引入的文件名 | `UISValue` | 通常由`tex=`引入该属性|
| `UISFrameFile` | 帧文件名 | 表示一串文件名前缀相同，但后缀由数字递增的文件 | `UISValue` | 通常由`frame=`引入该属性 |
| `UISHexColor` | 颜色 | 表示一个以`#`开头的，十六进制RGB表示的颜色 | `UISValue` | 通常由`color=`引入该属性 |
| `UISMotion` | 动画 | 指向一个动画定义，并管理播放延迟`Delay` | `UISValue` | 通常由`motion=`引入 |
|立即值与字面值|
| `UISNumber` | 数字 | 使用双精度`double`存储的一个数字 | `UISLiteralValue` | 有直接对应`double`的隐式转换，可以在需要传递double的场合直接传入本类 |
| `UISPercent` | 百分比 | 表示基于父容器的百分比的数值 | `UISLiteralValue` | |
| `UISPixel` | 像素 | 以一串数字表示的像素长度 |  `UISLiteralValue` | |
| `UISCurve` | 曲线 | 表示一串曲线，可以由一系列点的集合组成，也可以是预制曲线 | `UISLiteralValue` | 预制曲线只支持`EASEIN`和`EASEOUT` |
| `UISSimpleExpr` | 简单表达式 | 支持`+`,`-`的表达式组合，可以计算百分比，像素，和数字 | `UISLiteralValue` | 在含有百分比计算的情况下，会以父容器的值为基准 |
| `UISAnimationTime` | 动画时间 | 表示一段动画在时间轴上开始和结束播放的时间 | `UISLiteralValue` | 通常由`time=`引入 |
| `UISAnimationCurve` | 动画曲线 | 表示动画运行的曲线，管理一个`UISCurve`实例 | `UISLiteralValue` | 通常由`trans=`引入 |
| `UISAnimationRepeat`| 动画重复 | 表示动画重复与重复之间的延迟，格式如 `rA, B` | `UISLiteralValue` | 通常由`repeat=`引入 |
| `UISText` | 文本 | 表示一串文本 | `UISLiteralValue` | 无 |
| `UISRect` | 区域 | 使用左上角点`(x, y)`和区域的长宽`(w, h)` 确定的一个区域 | `UISLiteralValue` | `Scale9`大概会用到这个属性 |

-----------------------

### UISEditor渲染流程

1. 在解析完成后，调用`RenderManager.Render()`，立即生成渲染树
2. `Render()`将会遍历当前`UISObjectTree`的实例
3. 枚举所有`UISList`判断是否是`UISCustomElement`或`UISPredefineElement`列表

	如果是`UISCustomElement`则会判断`type`属性，并根据不同的`type`生成不同的渲染实例(`UISCustomRenderable`)。

	如果是`UISPredefineElement`则直接根据元素名称`ElementName`来生成渲染类实例(`UISPredefineRenderable`)。
4. 所有生成的类实例将会被直接缓存到内存中，再次调用Render()不会再生成新的实例，而是根据缓存，调用

	`RefreshProperties` 和 `ApplyProperties` 函数，此时渲染实例将会自动根据当前运行状态，应用新的属性。

5. 所有渲染实例所持有的渲染图层`Canvas`也会被缓存，无须担心设备丢失问题。
6. 渲染所需资源由`ResourceManager`（图像等）直接管理。

### UISEditor资源管理器

- 所有资源将会被永久缓存，直到退出软件为止

- 使用`GUID`标志每个资源，也可以使用**绝对路径**来访问资源	

- 每个绝对路径对应的资源只会被加载一次，多次加载将立即返回缓存

- 在资源调用时会判断对应实体文件是否更改，如果有修改则会从文件中更新

- 文件查找则是优先从本地查找，如果没有结果则从`plist`中查找

- `plist`中的全部图像资源在`plist`文件加载时立即全部缓存