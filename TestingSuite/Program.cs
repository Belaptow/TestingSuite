// See https://aka.ms/new-console-template for more information
using System.Threading.Tasks;
using TaskBroker;
using SharedLib;
using System.ComponentModel.DataAnnotations;


Console.WriteLine("Hello, World!");


var splitPagesTest = new List<int>() { 1, 1, 4, 1, 1, 5 }; //{ 5, 6, 9, 4, 3, 9, 6, 2, 5 };//
var outliers = new List<int>();
var strict = splitPagesTest.SplitPagesAggregateStrict(0, (acc, entry) => acc += entry, (acc) => acc <= 10, out outliers);
var looselyEqual = splitPagesTest.SplitPagesLooselyEqual(entry => entry);
//var looselyEqualWitLimit = splitPagesTest.SplitPagesLooselyEqual(entry => entry, 8);

Console.WriteLine(
    $"strict [{String.Join(", ", strict.Select(sub => $"[{String.Join(", ", sub.Select(i => i.ToString()))}]"))}]\n" +
    $"outliers [{String.Join(", ", outliers.Select(i => i.ToString()))}]\n" +
    $"looselyEqual [{String.Join(", ", looselyEqual.Select(sub => $"SUM({sub.Sum()})[{String.Join(", ", sub.Select(i => i.ToString()))}]"))}]\n" 
    //$"looselyEqualWitLimit [{String.Join(", ", looselyEqualWitLimit.Select(sub => $"SUM({sub.Sum()})[{String.Join(", ", sub.Select(i => i.ToString()))}]"))}]\n"
    );

//669da8fc-5de5-4db8-930a-85846534adac
var minGuid = Guid.Empty;
var maxGuid = new Guid(Enumerable.Repeat(byte.MaxValue, 16).ToArray());

var guid = Guid.Parse("669da8fc-5de5-4db8-930a-85846534adab");//Guid.NewGuid();
var incremented = guid.Increment();
var decremented = guid.Decrement();
var rollback = incremented.Decrement();

Console.WriteLine($"source {guid} incremented {incremented} decremented {decremented} EQ? {Equals(guid, rollback)}");

var incrementedMax = maxGuid.Increment();
var decrementedMin = minGuid.Decrement();
Console.WriteLine($"max {maxGuid} min {minGuid} incrementedMax {incrementedMax} decrementedMin {decrementedMin}");



var logger = new ActionLogger(Console.WriteLine, Console.WriteLine);

var limit = 10;
var broker = new BrokerService(logger);

var queueTasks = new List<Task>();
var source = new CancellationTokenSource();

var queues = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() };
queues.ForEachAsync(
    queue =>
        Enumerable.Range(0, limit).ForEachAsync(
            test =>
                broker.EnqueueAndWait(queue, () =>
                {
                    var timeout = new Random().Next(1, 5);
                    logger.DebugFormat("doin work for {0} sec.", timeout);
                    Thread.Sleep(timeout * 1000);
                    return true;
                }, TimeSpan.FromSeconds(10000))
            )
    );
Console.WriteLine("all finished");