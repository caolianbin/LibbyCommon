
Console.WriteLine("****************************");

#region 取出字符串中的所有字母，并转换成小写，统计每个字符出现的次数进行分组排序。显示次数大于1的字符统计
//string str = "skhslk,sklsdlk3,mskguiw";
//var items = str.Where(s => char.IsLetter(s)) //取出字母
//    .Select(s => char.ToLower(s)) //将字母转换成小写
//     .GroupBy(s => s) //进行分组
//     .Select(s => new { s.Key, count = s.Count() }) //s.key 为分组Key，统计次数
//     .OrderByDescending(s => s.count)//按照次数进行排序
//     .Where(s => s.count > 1);//条件
//foreach (var item in items)
//{
//    Console.WriteLine(item);
//}
#endregion

#region 取最大值和最小值
//int a = 5, b = 8, c = 9;
//int max = Math.Max(a, Math.Max(b, c));
//int min = Math.Min(a,Math.Min(b,c));
//Console.WriteLine($"最大值为：{max} ,最小值为{min}");
#endregion

#region 手写Lamba表达式的自定义方法
//List<int> nums = new List<int>() { 1,5,69,10,15,16,41,100};
//IEnumerable<int> result = MyWhere(nums, a => a > 10);
//foreach (var r in result)
//{
//	Console.WriteLine($"结果：{r}");
//}
//static IEnumerable<int> MyWhere(IEnumerable<int> items,Func<int,bool> f) 
//{
//    List<int> list = new List<int>();
//	foreach (var item in items)
//	{
//		if (f(item))
//		{
//			list.Add(item);
//		}
//	}
//	return list;
//}
#endregion

#region 随机排序
//List<int> nums = new List<int>() { 1,5,69,10,15,16,41,100};
//Random rand = new Random();
//var items = nums.OrderByDescending(e => rand.Next());
////var items = nums.OrderByDescending(e => Guid.NewGuid());
//foreach (var item in items)
//{
//    Console.WriteLine(item);
//}
#endregion







Console.WriteLine("****************************");

