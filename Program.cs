using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GiftsChecker
{
    class Program
    {
        static Regex rgxp;
        static int ChangeRegex
        {
            set
            {
                if (value == 1)
                    rgxp = new Regex("humblebundle.com.gift.key=[A-z0-9]{1,16}");
                else if (value == 2)
                    rgxp = new Regex("humblebundle.com..key=[A-z0-9]{1,16}");
                else if (value == 3)
                    rgxp = new Regex("humblebundle.com..gift=[A-z0-9]{1,16}");
                else if (value == 4)
                    rgxp = new Regex("humblebundle.com.s.key=[A-z0-9]{1,16}");
            }
        }
        static long Indx = 0;
        static long AllStrings = 0;

        static void Main(string[] args)
        {
            Console.Title = "GiftsChecker ~ v0.0.1";
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("File with urls -> ");
            var file = Console.ReadLine();

            if (file.StartsWith("\""))
                file = file.Replace("\"", "");

            Console.WriteLine();
            AllStrings = File.ReadLines(file).Count();

            using (HttpRequest req = new HttpRequest())
            {
                HttpResponse response;
                MatchCollection matchs;
                string StringResponse = string.Empty;
                req.AllowAutoRedirect = false;
                req.IgnoreProtocolErrors = true;

                foreach (var line in File.ReadLines(file))
                {
                    Console.Title = "GiftsChecker ~ v0.0.1 ~ " + Indx + "/" + AllStrings;

                    //?gift?key=HT5dhTWTAvWYdKMT
                    ChangeRegex = 1;
                    matchs = rgxp.Matches(line);

                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                        {
                            if (match.Success)
                            {
                                var value = match.Value;

                                if (value.EndsWith("If"))
                                    value = value.Replace("If", "");

                                response = req.Get("https://www." + value);
                                if (response.Location.Contains("receipt?s="))
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    Console.WriteLine("https://www." + value);
                                    continue;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    Console.WriteLine("https://www." + value + " | Name: " + response.ToString().Substring("\"giftName\": \"", "\""));
                                    continue;
                                }
                            }
                        }
                    }

                    //?key=HT5dhTWTAvWYdKMT
                    ChangeRegex = 2;
                    matchs = rgxp.Matches(line);

                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                        {
                            if (match.Success)
                            {
                                var value = match.Value;

                                if (value.EndsWith("If"))
                                    value = value.Replace("If", "");

                                response = req.Get("https://www." + value);
                                if (response.Location.Contains("downloads?key="))
                                {
                                    response = req.Get(response.Location);
                                    if (response.Location.Contains("receipt?s="))
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("https://www." + value);
                                        continue;
                                    }
                                    else if (response.ToString().Contains("Humble Bundle - Key already claimed"))
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("https://www." + value);
                                        continue;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                                            StringResponse = response.ToString();
                                            if (StringResponse.Contains("Thanks for purchasing"))
                                                Console.Write("https://www." + value + " | Name: " + StringResponse.Substring("Thanks for purchasing ", "<div"));
                                            else
                                                Console.Write("https://www." + value + " | Name: PartialGift");
                                            continue;
                                        }
                                        finally
                                        {
                                            value = value.Remove(0, value.IndexOf('=')+1);
                                            StringResponse = req.Get("https://www.humblebundle.com/api/v1/order/" + value + "?wallet_data=true&all_tpkds=true").ToString();
                                            if (StringResponse.Contains("\"sold_out\":false"))
                                                Console.Write(" | Unactivated keys: +" + Environment.NewLine);
                                            else
                                                Console.Write(" | Unactivated keys: -" + Environment.NewLine);

                                            req.AddHeader("x-requested-with", "XMLHttpRequest");

                                            var cookieStore = new CookieStorage();
                                            cookieStore.Set("csrf_cookie", "JKmC1EDI9mii2VkJ-1-1553273985", ".humblebundle.com");
                                            cookieStore.Set("_simpleauth_sess", "eyJ1c2VyX2lkIjo2MTg4MjIxOTM3NjE0ODQ4LCJpZCI6ImRLdng5T0xYV3AiLCJhdXRoX3RpbWUiOjE1NTMyNzQwNDJ9|1553346092|4ab63ec4c7cb5bf75b74c54e10cf267d89139a33", ".humblebundle.com");
                                            cookieStore.Set("hbguard", "\"IbRVi5qNqcJaXQETuYYC4a97jUoc+BxYGNE6xwICSsc\075\"", ".humblebundle.com");

                                            req.Cookies = cookieStore;
                                            req.Post("https://www.humblebundle.com/bundle/claim", "claimkey=" + value, "application/x-www-form-urlencoded");
                                        }
                                    }
                                }
                                else
                                    throw new Exception(response.Location);
                            }
                        }
                    }

                    //?gift=HT5dhTWTAvWYdKMT
                    ChangeRegex = 3;
                    matchs = rgxp.Matches(line);
                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                        {
                            if (match.Success)
                            {
                                var value = match.Value;

                                if (value.EndsWith("If"))
                                    value = value.Replace("If", "");

                                response = req.Get("https://www." + value);
                                if (response.Location.Contains("gift?key="))
                                {
                                    response = req.Get(response.Location);
                                    if (response.Location.Contains("receipt?s="))
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("https://www." + value);
                                        continue;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Console.WriteLine("https://www." + value + " | Name: " + response.ToString().Substring("\"giftName\": \"", "\""));
                                        continue;
                                    }
                                }
                                else
                                    throw new Exception(response.Location);
                            }
                        }
                    }

                    //s?key=HT5dhTWTAvWYdKMT
                    ChangeRegex = 4;
                    matchs = rgxp.Matches(line);
                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                        {
                            if (match.Success)
                            {
                                var value = match.Value;

                                if (value.EndsWith("If"))
                                    value = value.Replace("If", "");

                                response = req.Get("https://www." + value);
                                if (response.Location.Contains("receipt?s="))
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    Console.WriteLine("https://www." + value);
                                    continue;
                                }
                                else
                                {
                                    StringResponse = req.Get(response.Location).ToString();
                                    if (StringResponse.Contains("Humble Bundle - Key already claimed"))
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("https://www." + value);
                                        continue;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Console.WriteLine("https://www." + value + " | Name: " + response.ToString().Substring("\"giftName\": \"", "\""));
                                        continue;
                                    }
                                }
                            }
                        }
                    }

                    Indx++;
                }
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("..End!");
            Console.ReadKey();
        }
    }
}
