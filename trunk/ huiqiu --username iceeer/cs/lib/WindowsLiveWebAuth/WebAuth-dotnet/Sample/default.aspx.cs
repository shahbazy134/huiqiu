using System;
using System.Web;
using System.IO;
using WindowsLive;

/// <summary>
/// This is the default aspx.cs page for the sample Web Auth site.
/// It gets the application ID and user ID for display on the main
/// page.  
/// </summary>
public partial class DefaultPage : System.Web.UI.Page
{
    const string LoginCookie = "webauthtoken";

    // Initialize the WindowsLiveLogin module.
    static WindowsLiveLogin wll = new WindowsLiveLogin(true);

    protected static string AppId = wll.AppId;
    protected string UserId;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        /* If the user token has been cached in a site cookie, attempt
           to process it and extract the user ID. */

        HttpRequest req = HttpContext.Current.Request;
        HttpCookie loginCookie = req.Cookies[LoginCookie];

        if(loginCookie != null){
            string token = loginCookie.Value;

            if (!string.IsNullOrEmpty(token))
            {
                WindowsLiveLogin.User user = wll.ProcessToken(token);

                if (user != null) 
                {
                    UserId = user.Id;
                }
            }
        }
    }
}
