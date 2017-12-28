using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet;
using WebApplication2.Models;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.IO;
using GoogleApi;
using VkNet.Utils;
using System.Threading;
using System.Text;
namespace WebApplication2.Services
{
    public class ParsingService: IParsingService
    {
        private  string Query;
        private List<Post> Com=new List<Post>();
        public async Task<Post[]> Common(string query, string Uni, string Fac)

        {
            Query =query;
            await prof(Uni, Fac);
            await Gazeta(DateTime.Now.AddDays(-21).Date.ToString("dd.MM.yy"), "");
            await vesti();
            await regnum();
          
            await news(Uni, Fac);
            
            await VK(50, 21, 0);
            await GetTweets(Uni, Fac);
            await ria(Query);
            

            return Com.ToArray();
            



        }
        public async Task  VK(long Count, int daysAgo, int daysTo)
        {
            Random rand = new Random();
           
            VkApi vk = new VkApi();
            Post[] posts;
            var unixTimestamp = DateTime.Now.AddDays(-daysAgo);
            var unixTimestamp2 = DateTime.Now.AddDays(-daysTo);


            ulong appID = 6267940;
            string email = "89234255908";
            string password = "TeamFour";

            if (!vk.IsAuthorized)
                vk.Authorize(new ApiAuthParams
                {
                    ApplicationId = appID,
                    Login = email,
                    Password = password
                });

            var search = vk.NewsFeed.Search(new VkNet.Model.RequestParams.NewsFeedSearchParams
            {
                Query = Query,
                Count = Count,
                StartTime = unixTimestamp,
                EndTime = unixTimestamp2
            });

            posts = new Post[search.Count];

            for (int i = 0; i < search.Count; i++)
            {
                Post post = new Post();
                /*if (search[i].Comments.Count != 0)
                {
                    var getComments = vk.Wall.GetComments(new VkNet.Model.RequestParams.WallGetCommentsParams
                    {
                        PostId = search[i].Id,
                        OwnerId = search[i].OwnerId,
                        NeedLikes = true,
                        Extended = true
                    });

                    if (search[i].Comments.Count != 0)
                    {
                        Comment[] comments = new Comment[getComments.Count];
                        for (int j = 0; j < getComments.Count; j++)
                        {
                            Comment comment = new Comment();
                            comment.text = getComments[j].Text;

                            var getName = vk.Users.Get(Math.Abs(search[i].FromId));
                            comment.ownersName = getName.FirstName + " " + getName.LastName;

                            comments[j] = comment;
                        }
                        post.comments = comments;
                    }

                }*/
                post.text = search[i].Text;
                var getNameForPost = vk.Users.Get(Math.Abs(search[i].OwnerId));
                post.ownersName = getNameForPost.FirstName + " " + getNameForPost.LastName;
                post.link = "https://vk.com/wall" + search[i].OwnerId + "_" + search[i].Id;
                post.date = search[i].Date.ToString();
                
                post.source = "vk";
                post.emo = emotion(post.text, post.source);
                post.pass = true;
                posts[i] = post;
                Thread.Sleep(1);
            }
            Com.AddRange(posts);
        }
        // профессор рейтинг
        public async Task prof(string uni, string fac)
        {
            CancellationTokenSource cts; 
            Random rand = new Random();
            var str = "";
            var newUrl = "";
            string url = "";
            List<Post> posts = new List<Post>();

            using (HttpClient client = new HttpClient())
            {
                if (fac != "")
                    url = "https://yandex.ru/search/?text=" + fac + "%20" + uni + "%20professorrating.org&lr=67";
                else
                {
                    url = "https://yandex.ru/search/?text=" + "Отзывы" + "%20" + uni + "%20professorrating.org&lr=67";
                }
                Uri uri;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri)) { var htmlCode = await client.GetStringAsync(url); Task.WaitAll();
                    str = htmlCode;
                }
                
                
            }


            string reg1;
            if (fac == "")
                reg1 = @"(professorrating.org/university.php.id=\d{0,7})";
            else
                reg1 = @"(professorrating.org/faculty.php.id=\d{0,10})";

            Regex rgx1 = new Regex(reg1);
            newUrl = "https://" + rgx1.Match(str).Groups[1].Value;

            using (HttpClient client = new HttpClient())
            {

                string htmlCode = await client.GetStringAsync(newUrl);
                Task.WaitAll();
                str = htmlCode;
            }

            string reg2 = @"user-name..(\n[ ]{32,35})?([\s\S]{0,20})([ ]{28,50})?\<a rel.{40,60}href..(.{7,15}).\>#[\s\S]{10,300}date.comment.\>(.{2})(.nbsp.|[ ]{0,1})(.{3,10}).nbsp.(.{4}).{0,20}p[\s\S]{500,650}comment..(\n[ ]{30,50})?([\s\S]{0,1000})([ ]{30,50})?\</p\>\n([ ]{0,60}\n)?\<form method..post";
            Regex rgx2 = new Regex(reg2);
            DateTime from = DateTime.Now.AddDays(-21);
            DateTime to = DateTime.Now;
            foreach (Match match in rgx2.Matches(str))
            {
                Post post = new Post();
                var Date = Convert.ToDateTime(match.Groups[5].Value + " " + match.Groups[7].Value + " " + match.Groups[8].Value);

                if (Date.CompareTo(from) > 0 && Date.CompareTo(to) < 0)
                {
                    post.date = Date.ToShortDateString();
                    post.link = newUrl + match.Groups[4].Value;
                    post.text = match.Groups[10].Value;
                    post.ownersName = match.Groups[2].Value;
                   
                    post.source = "prof";
                    post.emo = emotion(post.text, post.source);
                    post.pass = true;
                    Thread.Sleep(1);
                    posts.Add(post);
                }
            }
            Com.AddRange(posts);
        }
        public async Task news(string uni, string fac)
        {
            Random rand = new Random();
            var str = "";
            string url = "";
            List<Post> posts = new List<Post>();
            DateTime from = DateTime.Now.AddDays(-21);
            DateTime to = DateTime.Now;

            using (HttpClient client = new HttpClient())
            {
                url = "https://search.newsru.com/?qry=" + uni + "%20" + fac + "&page_number=1&total_page=&фamount_id=&search_type=4&search_day_start=" + from.Day.ToString() + "&search_month_start=" + from.Month.ToString() + "&search_year_start=" + from.Year.ToString() + "&search_day_end=" + to.Day.ToString() + "&search_month_end=" + to.Month.ToString() + "&search_year_end=" + to.Year.ToString() + "&sort=2&main_nr=on&msk=on";
                var htmlCode = await client.GetStringAsync(url);
                Task.WaitAll();
                str = htmlCode;
            }
            string col;
            string reg1 = @"Найдено записей: (\d*)";
            Regex rgx1 = new Regex(reg1);
            var match1 = rgx1.Match(str);
            if (match1.Groups[1].Value == "")
                return;
            col = match1.Groups[1].Value;
            int i = 0;
            int pageNumber = 0;
            while (pageNumber * 15 < Convert.ToInt32(col) && pageNumber < 4)
            {
                pageNumber++;
                if (pageNumber != 1)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        url = "https://search.newsru.com/?qry=" + uni + "%20" + fac + "&page_number=" + pageNumber + "&total_page=&фamount_id=&search_type=4&search_day_start=" + from.Day.ToString() + "&search_month_start=" + from.Month.ToString() + "&search_year_start=" + from.Year.ToString() + "&search_day_end=" + to.Day.ToString() + "&search_month_end=" + to.Month.ToString() + "&search_year_end=" + to.Year.ToString() + "&sort=2&main_nr=on&msk=on";
                        var htmlCode = await client.GetStringAsync(url);
                        Task.WaitAll();
                        str = htmlCode;
                    }
                }
                string reg2 = @"index-news-content[\s\S]{0,15}href..(http.*newsru.com.*). class.*title..\n[ ]*(.*)[\s\S]*?index.news.date..\n[ ]*(.*) г";
                Regex rgx2 = new Regex(reg2);
                foreach (Match match in rgx2.Matches(str))
                {
                    Post post = new Post();
                    var Date = Convert.ToDateTime(match.Groups[3].Value);
                    post.date = Date.ToShortDateString();
                    post.link = match.Groups[1].Value;
                    post.text = match.Groups[2].Value;
                    post.ownersName = "noname";
                    
                    post.source = "news.ru";
                    post.emo = emotion(post.text, post.source);
                    post.pass = true;
                    Thread.Sleep(1);
                    posts.Add(post);
                    i++;
                }
            }
            Com.AddRange(posts);
        }
        // вести
        public async Task vesti()
        {
            Random rand = new Random();
            var str = "";

            DateTime from = DateTime.Now.AddDays(-21);
            DateTime to = DateTime.Now;
            List<Post> posts = new List<Post>();
            using (HttpClient client = new HttpClient())
            {

                string url = "http://www.vesti.ru/search/index/?q=" + Query + "&news=0&video=0&date_start=" + from + "&date_end=" + to;
                string htmlCode = await client.GetStringAsync(url);
                str = htmlCode;
            }

            string pattern = @"search-item__date__day.\>(.{10,20})\<.p[\s\S]{0,150}(\<a href[\s\S]{30,100}\</a\>[\s\S]{10,50})?href=.(.{10,20}).\>(([\s\S]{0,100}).span class.{0,50}span.)?([\s\S]{0,200})\</a\>[\s\S]{0,10}\</h3\>";
            Regex rgx = new Regex(pattern);
            foreach (Match match in rgx.Matches(str))
            {
                Post post = new Post();

                post.date = match.Groups[1].Value;
                post.link = "http://www.vesti.ru" + match.Groups[3].Value;
                if (match.Groups[4].Value == "")
                    post.text = match.Groups[6].Value;
                else
                    post.text = match.Groups[5].Value + Query + match.Groups[6].Value;
                post.ownersName = "noname";
                
                post.source = "vesti";
                post.emo = emotion(post.text, post.source);
                post.pass = true;
                Thread.Sleep(1);
                posts.Add(post);
            }
            Com.AddRange(posts);
        }

        //  регнум

        public async Task regnum()
        {
            Random rand = new Random();
            var str = "";

            List<Post> posts = new List<Post>();
            using (HttpClient client = new HttpClient())
            {

                string url = "https://regnum.ru/news/search/" + Query + ".html";
                var result = await client.GetStringAsync(url);
                Task.WaitAll();
                str = result.ToString();
            }
            string pattern = @"\<a href=.(.{40,50}). class=.news-container[\s\S]{200,420}data-location.{40,50}\>[\s\S]{17}(.{8})[\s\S]{327}([\s\S]{0,200})[ ]{16}\</span";
            Regex rgx = new Regex(pattern);
            DateTime from = DateTime.Now.AddDays(-21);
            DateTime to = DateTime.Now;
            foreach (Match match in rgx.Matches(str))
            {
                Post post = new Post();
                var Date = Convert.ToDateTime(match.Groups[2].Value);
                if (Date.CompareTo(from) > 0 && Date.CompareTo(to) < 0)
                {
                    post.date = Date.ToShortDateString();
                    post.link = match.Groups[1].Value;
                    post.text = match.Groups[3].Value;
                    post.ownersName = "noname";
                   
                    post.source = "regnum";
                    post.emo = emotion(post.text, post.source);
                    post.pass = true;
                    Thread.Sleep(1);

                    posts.Add(post);
                }
            }
            Com.AddRange(posts);
        }

    public async Task Gazeta( string from, string to)
        {
            Random rand = new Random();
            var str = "";

            List<Post> posts = new List<Post>();
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetStringAsync("https://www.gazeta.ru/search.shtml?p=search&page=0&text=" + Query + "&article=&section=&from=" + from + "&to=" + to + "&sort_order=published_desc&input=utf8");
                Task.WaitAll();
                str = result.ToString();
            
            }
            
            string pattern = @"pubdate=.(.{10})[\s\S]{0,100} 5px;..\n\<a href=.(/\w+/news/.{0,50}shtml)[\s\S]{0,20}\>\n([\s\S]{0,300})\n\</a\>\n\</h2>([\s\S]{0,4000} s_author_name..(.{0,50})..span)?";
            Regex rgx = new Regex(pattern);

            foreach (Match match in rgx.Matches(str))
            {
                Post post = new Post();

                post.date = match.Groups[1].Value;
                post.link = "https://www.gazeta.ru/" + match.Groups[2].Value;
                post.text = match.Groups[3].Value;

                if (match.Groups[4].Value != "")
                    post.ownersName = match.Groups[5].Value;
                else
                    post.ownersName = "noname";
                
                post.source = "gazeta";
                post.pass = true;
                post.emo = emotion(post.text, post.source);
                Thread.Sleep(1);
                posts.Add(post);
            }

            Com.AddRange(posts);
        }
        static async Task<string> GetAccessToken()
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token ");
            var customerInfo = Convert.ToBase64String(new UTF8Encoding()
                                      .GetBytes("NW0gawF1dfhzcjde3fTJNIpdj" + ":" + "c3WTSh5Wtxjrji8teqbak0ysm1hGkbk75poYKWSsPHHI6OLWhh"));
            request.Headers.Add("Authorization", "Basic " + customerInfo);
            request.Content = new StringContent("grant_type=client_credentials",
                                                    Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = await httpClient.SendAsync(request);
            string str = await response.Content.ReadAsStringAsync();
            string reg1 = @"access_token...(.*).}";
            Regex rgx1 = new Regex(reg1);
            var match = rgx1.Match(str);
            return match.Groups[1].Value;
        }


        public async Task GetTweets(string uni, string fac) // вызывать его!
        {
            Random rand = new Random();
            var accessToken = await GetAccessToken();
            List<Post> posts = new List<Post>();
            var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get,
                string.Format("https://api.twitter.com/1.1/search/tweets.json?q=" + uni + "%20" + fac + "&count=100&result_type=recent&include_entities=false"));

            requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
            var httpClient = new HttpClient();
            HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
            string str = await responseUserTimeLine.Content.ReadAsStringAsync();
            str = Regex.Replace(str, @"\\u([0-9A-Fa-f]{4})", m => "" + (char)Convert.ToInt32(m.Groups[1].Value, 16));
            string reg2 = @"coordinates[\s\S]*?created_at.{6} (.{6})[\s\S]*?id_str...(\d*).*?.{10}([\s\S]*?)...truncated[\s\S]*?,.name...(.*?)...screen_name...(.*?)...location";
            Regex rgx2 = new Regex(reg2);
            foreach (Match match in rgx2.Matches(str))
            {
                Post post = new Post();
                var Date = Convert.ToDateTime(match.Groups[1].Value + " 2017");
                post.date = Date.ToShortDateString();
                post.link = "https://twitter.com/" + match.Groups[5] + "/status/" + match.Groups[2].Value;
                post.text = match.Groups[3].Value;
                post.ownersName = match.Groups[4].Value;
                
                post.source = "twitter";
                post.emo = emotion(post.text, post.source);
                post.pass = true;
                Thread.Sleep(1);
                posts.Add(post);
            }
            Com.AddRange(posts);
        }
        public async Task ria(string Query)
        {
            Random rand = new Random();
            var str = "";
            DateTime from = DateTime.Now.AddDays(-21);
            List<Post> posts = new List<Post>();
            int i = 0;
            while (i < 5)
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://ria.ru/services/search/getmore/?offset=" + i * 20 + "&query=" + Query;
                    string htmlCode = await client.GetStringAsync(url);
                    str = htmlCode;
                }

                string pattern = @"href=.(.*?html)[\s\S]*?item-title[\s\S]*?span.([\s\S]*?)..span( class..search.highlight..[\s\S]*?span.([\s\S]*?)..span)?[\s\S]*?item-date[\s\S]*?span.([\s\S]*?)..span";
                Regex rgx = new Regex(pattern);
                foreach (Match match in rgx.Matches(str))
                {
                    Post post = new Post();

                    if (Convert.ToDateTime(match.Groups[5].Value).CompareTo(from) < 0)
                        return;
                    post.date = match.Groups[5].Value;
                    post.link = "https://ria.ru" + match.Groups[1].Value;
                    post.text = match.Groups[2].Value;
                    if (match.Groups[3].Value != "")
                        post.text += " " + Query + match.Groups[4].Value;
                    post.ownersName = "noname";
                    post.source = "ria";
                    post.emo = emotion(post.text, post.source);
                    
                    post.pass = true;
                    posts.Add(post);
                    
                }
                i++;
            }
            Com.AddRange(posts);
        }
        public int emotion(string text, string source)
        {
            Random rand = new Random();
            switch (source)
            {
                case "ria":
                case "news":
                case "gazeta":
                case "regnum":
                    return 2;
                    break;
                default:
                    if (text.Length > 400) return 3;
                    else
                    {
                        if (text.Contains("Уважаемый")) return 0;
                        if (text.Contains('!')) return rand.Next(2);
                        if (text.Contains(')') && (source=="vk"|| source =="twitter")) return 0;
                        return 2;
                    }
                        break;
            }


        }

        public string getstr(string str) // new
        {


            string reg1 = @".div class..right-part..[\s\S]*.form.[\s\S]*?div.";
            Regex rgx1 = new Regex(reg1);
            str = rgx1.Replace(str, " ");
            reg1 = @".div class..menu..[\s\S]*?.form.[\s\S]*?div.";
            rgx1 = new Regex(reg1);
            str = rgx1.Replace(str, " ");
            reg1 = @"/\*";
            rgx1 = new Regex(reg1);
            str = rgx1.Replace(str, " ");
            reg1 = @"\*/";
            rgx1 = new Regex(reg1);
            str = rgx1.Replace(str, " ");
            reg1 = @".div class..footer-buttons..[\s\S]*?div.";
            rgx1 = new Regex(reg1);
            str = rgx1.Replace(str, " ");
            return str;
        }

    }
}

