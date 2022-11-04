// See https://aka.ms/new-console-template for more information
using Common.Partials;

Console.WriteLine("Hello, World!");

Users u = new Users();
var a = u.GetString();

Console.WriteLine($"值：{a}");

