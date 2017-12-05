using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication2.Models;
using WebApplication2.Models.Requests;
using WebApplication2.Data;
using WebApplication2.Models.AccountViewModels;
using WebApplication2.Services;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly string _externalCookieScheme;
        private readonly ApplicationDbContext context;
        private readonly IParsingService parsingService;

        public RequestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            ApplicationDbContext context,
            IParsingService parsingService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            this.context = context;
            this.parsingService = parsingService;
        }



        
        public async Task<IActionResult> Choose()
        {
            
            var Univer = await this.context.Universities.ToListAsync();
            var Faculty= await this.context.Faculties.ToListAsync();




            var model = new Choose
            {
                Universities = (ICollection<University>)Univer,
                Faculties = (ICollection<Faculty>)Faculty


            };

            return this.View(model);

            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Choose(Choose model, string un, string fa, bool locker)
        {
            if (!locker)
            {
                var Univer = await this.context.Universities.ToListAsync();
                var Faculty = await this.context.Faculties.ToListAsync();
                var model1 = new Choose
                {
                    Universities = (ICollection<University>)Univer,
                    Faculties = (ICollection<Faculty>)Faculty,
                    University = un


                };
                this.ViewBag.University = un;
                return this.View(model1);
            }
            
            Post[] posts =await parsingService.Common(un + " " + fa);
            
                var itemsToDelete = context.Set<Post>();
                context.Posts.RemoveRange(itemsToDelete);
                await context.SaveChangesAsync();
            
            foreach (var postitem in posts)
            {
                var post = new Post
                {
                    ownersName=postitem.ownersName,
                    text=postitem.text,
                    date=postitem.date,
                    comments=postitem.comments,
                    link=postitem.link
                };
                context.Posts.Add(post);
            }
            await this.context.SaveChangesAsync();

            var Postmass = await this.context.Posts.ToListAsync();
            return this.View("Blacklist", new List
                 {
                     University=un,
                     Faculty=fa,
                     Query=un+" "+fa,
                     Posts=(ICollection<Post>)Postmass
                 });
        }

    


    }
}
