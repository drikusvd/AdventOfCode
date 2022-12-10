using System.Reflection;
using AdventOfCode2022;

var baseType = typeof(ProcessorBase);
var processors = Assembly.GetEntryAssembly().GetTypes().Where(a => a.IsSubclassOf(baseType)).ToList();
processors.Sort((a, b) =>
{
    var aDay = int.Parse(a.Namespace.Substring(a.Namespace.LastIndexOf(".") + 1).Replace("Day", ""));
    var bDay = int.Parse(b.Namespace.Substring(b.Namespace.LastIndexOf(".") + 1).Replace("Day", ""));

    return aDay - bDay;
});

foreach (var processor in processors)
{
    await ((ProcessorBase)Activator.CreateInstance(processor)).Process();
}
