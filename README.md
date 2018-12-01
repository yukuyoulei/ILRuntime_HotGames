# ILRuntime_HotGames
基于ILRuntime的热更新能力实现的可以直接使用的框架。

AHotGames是C#热更项目。
UHotGames是Unity项目。

C#热更项目（AHotGames）请用VS2017打开。
Unity项目是用Unity2018创建的，不过2017版本以上的Unity应该也能打开，并没有用到什么高级特性，只是一堆代码，目前只有Scene/Main场景有用，其实自己创建一个空场景，随便在哪个GameObject挂上入口类Enter类就可以跑了。

使用方法很简单，ILRuntime部分已经在Unity工程中整合，除非有未实现的ILRuntime适配器需要添加，或者ILRuntime有重大更新，否则不建议修改这部分。在C#热更项目中写好功能后编译，我已经写好编译后事件，VS会直接将生成到Unity项目的dll的扩展名修改成bytes，以避免Unity将热更dll直接编译入最终的Assemble中。
Unity项目中的Enter类为起始类，可以修改Config路径为自己的远程路径。
Unity项目中的UBuildTools类为编辑器辅助类，在Unity编辑器中运行，可以打最终包，也可以打AssetBundle包。

在C#热更项目部分新加的类建议都从AGameBase继承，这样可以直接使用很多基类方法。

已经实现的几个GUI游戏中发现一些小问题，原因是Unity的一些内置资源在热更项目里面无法直接获取，之后我会再想办法，不过想必身处8012年末期的大家应该也不会再用GUI来做游戏的，所以这个坑不是很着急填。

AGameBase类不是从MonoBehaviour继承的，ILRuntime的原作者建议热更项目中尽量不要继承自MonoBehaviour，所以我也就这么做了。 Update和OnGUI这两个需要每帧执行的方法似乎比较损耗性能，各位开发时也需要慎重，当然，亲测一些小游戏是完全没必要在意这些小损耗的。

ILRuntime项目地址：
https://github.com/Ourpalm/ILRuntime
向大佬致敬，感谢大佬带来了可以让我们用C#热更功能的ILRuntime。

目前已知限制：
 - 不能使用? 
	示例： int? ivalue = null;
	这种用法暂时是无法使用的，会报错。