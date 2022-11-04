// See https://aka.ms/new-console-template for more information
using Common.Partials;

Console.WriteLine("Hello, World!");

//局部类使用 
Users u = new Users();
var a = u.GetString();

Console.WriteLine($"值：{a}");

