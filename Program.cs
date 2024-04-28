using System.Diagnostics;

namespace Study06Parallel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CountSpaceInThreeFiles();

            Console.WriteLine(string.Empty);

            CountSpacesInAllFilesOfDirectory(new DirectoryInfo(path: AppContext.BaseDirectory));
        }

        static void CountSpaceInThreeFiles()
        {
            var path = AppContext.BaseDirectory;
            //up to solution directory
            path = Path.GetDirectoryName(path);
            path = Path.GetDirectoryName(path);
            path = Path.GetDirectoryName(path);
            path = Path.GetDirectoryName(path);

            var files = new[] { 
                new FileInfo(Path.Combine(path,"Program.cs")), 
                new FileInfo(Path.Combine(path,"Study06Parallel.csproj")), 
                new FileInfo(Path.Combine(path,"Study06Parallel.sln")) 
            };

            Console.WriteLine(nameof(CountSpaceInThreeFiles));
            var stopwatch=Stopwatch.StartNew(); 

            Task.WaitAll(files.Select(CountSpace).ToArray());

            stopwatch.Stop();
            Console.WriteLine($"all ticks: {stopwatch.ElapsedTicks}");              
        }

        static async Task<int> CountSpace(System.IO.FileInfo file)
        {
            var stopwatch = Stopwatch.StartNew();

            int result = 0;
            var text = await System.IO.File.ReadAllTextAsync(file.FullName);

            foreach(var ch in text) {
                if(ch == ' ') { 
                    result++; 
                };
            }
                        
            await Task.Delay(100);//for demonstration purpose; otherwise too fast

            stopwatch.Stop();
            Console.WriteLine($"{file.FullName}, spaces: {result}, ticks: {stopwatch.ElapsedTicks}");            

            return result;
        }

        static void CountSpacesInAllFilesOfDirectory(System.IO.DirectoryInfo directoryInfo)
        {            
            Console.WriteLine($"{nameof(CountSpacesInAllFilesOfDirectory)}, directory: {directoryInfo}");
            var stopwatch = Stopwatch.StartNew();

            var tasks = directoryInfo.EnumerateFiles().Select(CountSpace).ToArray();
            Task.WaitAll(tasks);
            int allSpaces = tasks.Aggregate(seed:0, (acc, task) => {return acc + task.Result;});

            stopwatch.Stop();
            Console.WriteLine($"all spaces: {allSpaces}, all ticks: {stopwatch.ElapsedTicks}");
        }
    }
}
