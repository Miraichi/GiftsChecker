using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GiftsChecker
{
    public class HumbleBundle
    {
        private Regex rgxp;
        private HttpResponse response;
        private MatchCollection matchs;
        private HashSet<string> DupChecker = new HashSet<string>();

        private string StringResponse = string.Empty;

        private int ChangeRegex
        {
            set
            {
                switch (value)
                {
                    case 1:
                        rgxp = new Regex("humblebundle.com.gift.key=[A-z0-9]{1,16}");
                        break;
                    case 2:
                        rgxp = new Regex("humblebundle.com..key=[A-z0-9]{1,16}");
                        break;
                    default:
                        break;
                }
            }
        }

        public int GetCountUrls()
        {
            return DupChecker.Count();
        }

        public void Search(string Line)
        {
            CheckGifts(Line);
            CheckBundles(Line);
        }

        private void CheckGifts(string Line)
        {
            try
            {
                using (HttpRequest req = new HttpRequest())
                {
                    req.AllowAutoRedirect = false;
                    req.UseCookies = false;
                    req.IgnoreProtocolErrors = true;
                    req.UserAgentRandomize();

                    Line = Line.Replace("?gift=", "gift?key=");
                    matchs = Parser(1, Line);

                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                        {
                            if (match.Success)
                            {
                                if (!DupChecker.Contains(match.Value))
                                {
                                    var value = match.Value;
                                    if (value.EndsWith("If"))
                                        value = value.Replace("If", "");

                                    response = req.Get("https://www." + value);
                                    if (response.Location.Contains("receipt?s="))
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("https://www." + value);
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Console.WriteLine("https://www." + value + " | Name: " + response.ToString().Substring("\"giftName\": \"", "\""));
                                    }

                                    DupChecker.Add(match.Value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Error: " + ex.Message + " || Method: CheckGifts");
            }
        }

        private void CheckBundles(string Line)
        {
            try
            {
                using (HttpRequest req = new HttpRequest())
                {
                    req.AllowAutoRedirect = false;
                    req.UseCookies = false;
                    req.IgnoreProtocolErrors = true;
                    req.UserAgentRandomize();

                    Line = Line.Replace("s?key=", "?key=").Replace("download?key=", "?key=");
                    matchs = Parser(2, Line);

                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                        {
                            if (match.Success)
                            {
                                if (!DupChecker.Contains(match.Value))
                                {
                                    var value = match.Value;
                                    value = value.Remove(0, value.IndexOf('=') + 1);

                                    response = req.Get("https://www.humblebundle.com/api/v1/order/" + value + "?all_tpkds=true");
                                    StringResponse = response.ToString();

                                    if (!response.IsOK || StringResponse.Contains("Unauthorized"))
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("https://www." + match.Value);
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        if (StringResponse.Contains("\"sold_out\":false"))
                                            Console.Write("https://www." + match.Value + " | Name: " + StringResponse.Substring("\"human_name\":\"", "\"") + " | Unactivated keys: +" + Environment.NewLine);
                                        else
                                            Console.Write("https://www." + match.Value + " | Name: " + StringResponse.Substring("\"human_name\":\"", "\"") + " | Unactivated keys: -" + Environment.NewLine);
                                    }

                                    DupChecker.Add(match.Value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Error: " + ex.Message + " || Method: CheckBundles");
            }
        }

        private MatchCollection Parser(int Method, string Line)
        {
            ChangeRegex = Method;
            return rgxp.Matches(Line);
        }
    }
}
