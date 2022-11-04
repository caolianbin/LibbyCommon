// See https://aka.ms/new-console-template for more information
using 缓存类;

Console.WriteLine("Hello, World!");



TimeSpan ts = TimeSpan.FromSeconds(3000);
var s = MemoryCacheHelper.Set("aaa","bbb", ts,true);
Console.WriteLine($"缓存==》{s}");

var a = MemoryCacheHelper.Get("aaa");
Console.WriteLine($"缓存内容==》{a}");
 