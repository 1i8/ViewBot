using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;

namespace ViewBot
{
    internal class Program
    {
        public static int Time = 30000; // Need to watch the video for atleast 30 seconds to count as a view (30 s = 30000 ms)
		public static int Count;
		public static string Link;

        static void Main(string[] args)
        {
			Console.Title = "ViewBot";

			if (File.Exists("proxies.txt"))
			{
				File.Delete("proxies.txt");
			}

			Console.WriteLine("YouTube Video URL:");
            Link = Console.ReadLine();

			Console.WriteLine("Views Count:");
			Count = int.Parse(Console.ReadLine());

			Console.WriteLine("Proxies type: SOCKS4 (1) HTTP (2):");
			if (Console.ReadLine() == "1")
            {
				var proxies = new WebClient();
				proxies.DownloadFile("https://api.proxyscrape.com/v2/?request=getproxies&protocol=socks4&timeout=10000&country=all", "proxies.txt");
			}
			else if (Console.ReadLine() == "2")
			{
				var proxies = new WebClient();
				proxies.DownloadFile("https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout=10000&country=all&ssl=all&anonymity=all", "proxies.txt");
			}

			if (!File.Exists("chromedriver.exe"))
			{
				Console.WriteLine("chromedriver isn't found. Download chromedriver and put it in the View Bot folder's path. Url: https://chromedriver.chromium.org/downloads");
			}
			else
            {
				Start();
            }

			Console.ReadLine();
		}

		static async void Start()
        {
			try
            {
				ChromeOptions options = new ChromeOptions();
				options.AddArgument("log-level=3");
				options.AddArguments("mute-audio");
				options.AddArgument("--headless");
				options.AddArgument("--silent");
				options.AddArguments("--disable-extensions");
				options.AddArguments("--disable-notifications");
				options.AddArguments("--disable-application-cache");
				options.AddArguments("--disable-gpu");
				options.AddArgument("--ignore-certificate-errors");
				options.AddUserProfilePreference("profile.default_content_setting_values.cookies", 2);

				foreach (int Times in Enumerable.Range(1, Count))
				{
					try
					{
						var proxies = File.ReadAllLines("proxies.txt");
						var random = new Random();
						var random2 = random.Next(0, proxies.Length - 1);
						var server = proxies[random2];

						Proxy proxy = new Proxy();
						proxy.Kind = ProxyKind.Manual;
						proxy.IsAutoDetect = false;
						proxy.HttpProxy = server;
						options.Proxy = proxy;

						Console.WriteLine("Switching Proxy Server to: " + server);

						IWebDriver driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options)
						{
							Url = Link
						};

						if (Count == Times)
                        {
							Console.Title = "ViewBot";
							Console.WriteLine("Done: " + Count + " views.");
							driver.Quit();
							return;
                        }

						WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

						wait.Until(ExpectedConditions.ElementIsVisible(By.Id("movie_player")));
						driver.FindElement(By.Id("movie_player")).SendKeys(Keys.Space);

						await Task.Delay(Time);

						driver.Quit();
						Console.Title = "ViewBot | Count: " + Times;
					}
					catch (Exception ex)
					{ 
						Console.WriteLine("Error: " + ex);
					}
				}
			}
			catch (Exception ex)
            {
				Console.WriteLine("Error: " + ex);
            }
		}

    }
}
