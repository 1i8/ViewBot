using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace ViewBot
{
    class Program
    {
        static int time = 30000;
        static int count;
        static string url;

        static void Main(string[] args)
        {
            Console.Title = "ViewBot | extatent";
            Console.WriteLine("GitHub: https://github.com/extatent");

            if (File.Exists("proxies.txt"))
            {
                File.Delete("proxies.txt");
            }

            Console.WriteLine("YouTube Video URL:");
            url = Console.ReadLine();

            Console.WriteLine("Views Count:");
            count = int.Parse(Console.ReadLine());

            Console.WriteLine("Proxy Type: SOCKS4 (1) HTTP (2):");

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
            else
            {
                var proxies = new WebClient();
                proxies.DownloadFile("https://api.proxyscrape.com/v2/?request=getproxies&protocol=socks4&timeout=10000&country=all", "proxies.txt");
            }

            if (!File.Exists("chromedriver.exe"))
            {
                Console.WriteLine("chromedriver isn't found. Download chromedriver and put it in the View Bot folder's path. URL: https://chromedriver.chromium.org/downloads");
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
                options.AddArguments("--disable-extensions");
                options.AddArguments("--disable-notifications");
                options.AddArguments("--disable-application-cache");
                options.AddArguments("--no-sandbox");
                options.AddArguments("--disable-dev-shm-usage");
                options.AddArguments("--disable-gpu");
                options.AddArgument("--ignore-certificate-errors");
                options.AddArguments("disable-infobars");
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--start-maximized");
                options.AddArgument("--headless");
                options.AddArgument("--silent");
                options.AddUserProfilePreference("profile.default_content_setting_values.cookies", 2);

                foreach (int times in Enumerable.Range(1, count))
                {
                    try
                    {
                        IWebDriver driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
                        driver.Url = url;

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

                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("movie_player")));
                        driver.FindElement(By.Id("movie_player")).SendKeys(Keys.Space);

                        await Task.Delay(time);

                        driver.Quit();

                        Console.Title = "ViewBot | Count: " + times;

                        if (count == times)
                        {
                            Console.Title = "ViewBot";
                            Console.WriteLine("Done. " + count + " views should be added.");
                            driver.Quit();
                            return;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Something went wrong. Make sure the Chrome version is the same as the chromedriver version.");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Something went wrong. Make sure the Chrome version is the same as the chromedriver version.");
            }
        }
    }
}
