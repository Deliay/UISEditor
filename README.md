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
|`UISAnimation`| 动画列表 | 用于动画声明的相关属性和值的列表 | `UISList<UISAnimationProperty>` |  管理`UISAnimationProperty` |
| `UISAnimationElement` | 动画属性列表 | 用于以`:`开头的一组动画定义的属性列表 | `UISElement<UISAnimation>` | 管理`UISAnimation` |
| `UISPredefineElement` | 预定义元素列表 | 用于以`@`开头的预定义元素的属性列表 | `UISElement<UISProperty>` |无 |
| `UISCustomElement` | 自定义元素列表 | 用于以`_`开头的自定义元素的属性列表 | ` UISElement<UISProperty>` |无 |
|属性实现|
| `UISNull` | 值 | 表示一个空元素的值 | `UISValue` |无 |
| `UISVector` | 值 | 表示由两个立即值(`UISLiteralValue`)组成的值 | `UISValue` | 通常一些二元属性会用到，例如`pos=(x,y)`,`size=(x,y)` |
| `UISRelativeVector` | 值 | 表示某两个立即值是依赖某个元素进行计算的 | `UISVector`| 通常在做一些依赖属性时会引入该属性 |
| `UISAnimationProperty` | 动画 | 动画元素的参数属性 | `UISValue` |无 |
| `UISFileName` | 文件名 | 表示一个引入的文件名 | `UISValue` | 通常由`tex=`引入该属性|
| `UISFrameFile` | 帧文件名 | 表示一串文件名前缀相同，但后缀由数字递增的文件 | `UISValue` | 通常由`frame=`引入该属性 |
| `UISHexColor` | 颜色 | 表示一个以`#`开头的，十六进制RGB表示的颜色 | `UISValue` | 通常由`color=`引入该属性 |
