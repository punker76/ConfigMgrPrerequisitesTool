﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfigMgrPrerequisitesTool
{
    class WebEngine
    {
        public string LinkName { get; set; }
        public string LinkValue { get; set; }

        private static string WildCardToRegular(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }

        public List<WebEngine> LoadWindowsADKVersions()
        {
            //' Construct new link list for all suupported Windows ADK versions
            List<WebEngine> linkList = new List<WebEngine>();

            //' Construct new html objects and load web site into document
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(@"https://developer.microsoft.com/en-us/windows/hardware/windows-assessment-deployment-kit");

            //' Determine link nodes that contains ADK in the innerText property
            IEnumerable<HtmlNode> linkNodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]").Where(node => node.InnerText.IndexOf("ADK") > -1);

            //' Process each link node and construct a new WebEngine object for each ADK object
            foreach (HtmlNode linkNode in linkNodes)
            {
                string linkNodeText = linkNode.InnerText.Replace(@"&nbsp;", "").Replace(@"\s{2,}", " ").Trim().ToString();

                if (Regex.IsMatch(linkNodeText, WildCardToRegular("*Get*Windows*ADK*for*Windows*10*")) == true)
                {
                    WebEngine link = new WebEngine { LinkName = linkNodeText.Substring(4), LinkValue = String.Format("https://{0}", (linkNode.GetAttributeValue("href", string.Empty).Replace("//",""))) };
                    linkList.Add(link);
                }
            }

            return linkList;
        }
    }
}
