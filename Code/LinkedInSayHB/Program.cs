using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LinkedInSayHB
{
    public class Program
    {
        private static int timeOutInSeconds = 8;
        private static int timeOutDelay = 3000; // milliseconds
        private static string email { get; set; }
        private static string password { get; set; }
        private static string message { get; set; }
        private static bool hasCustomMessage { get; set; }

        [STAThread]
        public static void Main(string[] args)
        {

            ValidateParams(args);
           
            using (IWebDriver driver = new ChromeDriver(Environment.CurrentDirectory))
            {
                driver.Navigate().GoToUrl("https://www.linkedin.com/login/");
                driver.Manage().Window.Maximize();
                driver.FindElement(By.Id("username"),timeOutInSeconds).SendKeys(email);
                driver.FindElement(By.Id("password"),timeOutInSeconds).SendKeys(password);
                driver.FindElement(By.CssSelector(".login__form_action_container > button"),timeOutInSeconds).Click();
                driver.Navigate().GoToUrl("https://www.linkedin.com/notifications/");
                
                Task.Delay(timeOutDelay).Wait();

                var countCelebrating = 0;
                bool checkSeeCelebrating = true;
                while (checkSeeCelebrating)
                {
                    try
                    {
                        driver.FindElement(By.XPath("//span[text() = \"See who's celebrating\"]"), timeOutInSeconds).Click();
                        checkSeeCelebrating = false;
                    }
                    catch (Exception e)
                    {
                        checkSeeCelebrating = true;
                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                        js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                        Task.Delay(timeOutDelay).Wait();
                        countCelebrating++;
                        if (countCelebrating > 20)
                        {
                            Console.WriteLine("End because button not found!");
                            return;
                        }
                    }
                }

                Task.Delay(timeOutDelay).Wait();
                var nameList = driver.FindElements(By.CssSelector("article a.nt-card__headline"),timeOutInSeconds);
                var alreadySent = driver.FindElements(By.CssSelector("article div.t-14.t-black--light"),timeOutInSeconds).ToArray().Select(item => item.Text);
                var buttonsToSayHappy = driver.FindElements(By.XPath("//span[text() = 'Say happy birthday']"),timeOutInSeconds);
                
                var c1 = nameList.Count;
                Console.WriteLine("Count Name List: " + c1);
                var c2 = alreadySent.Count();
                Console.WriteLine("Count Already Sended: " + c2);
                var c3 = buttonsToSayHappy.Count;
                Console.WriteLine("Count Buttons to say happy: " + c3);

                var index = 0;
                foreach (var item in nameList.ToArray())
                {
                    string name = "";
                    bool checkName = true; 
                    while(checkName){
                        try{
                            name = item.Text;
                            checkName = false;
                        } catch(StaleElementReferenceException e){
                            checkName = true;
                        }
                    }
                    
                    name = name.Replace("Wish", "");
                    name = name.Replace("a happy birthday", "");
                    name = name.Replace("(today)", "").Trim();
                    name = name.Replace("(yesterday)", "").Trim();
                    
                    var firsName = name.Substring(0,name.IndexOf(" ", StringComparison.Ordinal));

                    var existInList = alreadySent.Any(l => l.Contains(firsName));

                    if (!existInList)
                    {
                        
                        buttonsToSayHappy[index].Click();
                        
                        driver.FindElement(By.CssSelector("form.msg-form div.msg-form__contenteditable > p"), timeOutInSeconds)
                            .Click();
                        
                        driver.FindElement(By.CssSelector("form.msg-form div.msg-form__contenteditable > p"), timeOutInSeconds)
                            .SendKeys(" " + firsName + (hasCustomMessage ? " " + message.Trim() : " Best wishes!"));
                        
                        driver.FindElement(By.CssSelector("form.msg-form div.msg-form__contenteditable > p"), timeOutInSeconds)
                            .SendKeys(Keys.Enter);

                         Task.Delay(timeOutDelay).Wait();
                        
                        driver.FindElement(SelectorByAttributeValue("data-control-name","overlay.close_conversation_window"), timeOutInSeconds)
                            .Click();
                        
                        index++;
                        
                    }
                    
                    Task.Delay(timeOutDelay).Wait();
                    
                }
                
            }
           
        }

        private static void ValidateParams(string[] args)
        {
            Console.WriteLine("LinkedIn Send Happy Birthday");
            Console.WriteLine("Parameter1: LinkedIn Email");
            Console.WriteLine("Parameter2: LinkedIn Password");
            Console.WriteLine("Parameter3 (Optional): Custom Message");
            Console.WriteLine("Starting Validation...");
            
            if (args == null || !(args.Any()) || args.Count() < 2)
            {
                throw new ArgumentNullException("Needs to send email and password.");
            }

            email = args[0]; // Assign email.
            password = args[1]; // Assign password.
            hasCustomMessage = (args.Count() > 2); // Set has Custom Message if count is greater than 2.
            message = (hasCustomMessage ? args[2] : ""); // Set message if hast Custom Message is true.
            hasCustomMessage = !(hasCustomMessage && string.IsNullOrEmpty(message)); // set has Custom Message to false if has message is true and message is null.

            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                throw new ArgumentNullException("Needs to send a valid email");
            }
            
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Needs to send a valid password");
            }
             
            Console.WriteLine("Validation Completed..");
        }

        private static bool IsElementPresent(By by, IWebElement driver)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        
        private static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"); 
        }
        
        public static By SelectorByAttributeValue(string p_strAttributeName, string p_strAttributeValue)
        {
            return (By.XPath(String.Format("//*[@{0} = '{1}']", 
                p_strAttributeName, 
                p_strAttributeValue)));
        }
        
    }
}