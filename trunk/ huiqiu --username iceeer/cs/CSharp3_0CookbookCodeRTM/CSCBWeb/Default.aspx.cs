using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Configuration;
using System.Web.Caching;
using System.Data.SqlClient;
using System.Diagnostics;

public partial class Default_aspx : System.Web.UI.Page 
{
    // Page events are wired up automatically to methods 
    // with the following names:
    // Page_Load, Page_AbortTransaction, Page_CommitTransaction,
    // Page_DataBinding, Page_Disposed, Page_Error, Page_Init, 
    // Page_Init Complete, Page_Load, Page_LoadComplete, Page_PreInit
    // Page_PreLoad, Page_PreRender, Page_PreRenderComplete, 
    // Page_SaveStateComplete, Page_Unload

	string _connStr = "Persist Security Info=False;Integrated Security=SSPI;" +
	"database=pubs;server=localhost;Connect Timeout=30";

    protected void Page_Load(object sender, EventArgs e)
    {
		// if the initial request
		if (Request.QueryString.Count == 0)
		{
			// run 14.15 to add the sqlCache database entry 
			// to web.config
			TestConfig();
			// now redirect to ourselves adding a querystring
			// we do this so that the change we made to 
			// web.config gets picked up for the code in
			// CreateSqlCacheDependency and SetupCacheDependencies
			// as it depends on that configuration being present.

			// if you just create the entry and call the setup
			// code in the same page instance, the internal 
			// configuration stuff doesn't refresh and you get
			// an exception when the code can't find the sqlCache
			// section it needs.
			Response.Redirect(Request.RawUrl + "?run=1");
		}
		else
		{
			// run 14.10
			CreateSqlCacheDependency(_connStr);
			// run 14.11
			SetupCacheDependencies(_connStr);
		}
	}

	#region "14.10 Tying your database code to the cache"
	public SqlCacheDependency CreateSqlCacheDependency(string connStr)
	{
		// make a dependency on the authors database table so that 
		// if it changes, the cached data will also be disposed of

        // make sure we are enabled for notifications for the db
		// note that the parameter has to be the actual connection
		// string NOT the connection string NAME from web.config
		SqlCacheDependencyAdmin.EnableNotifications(connStr);
		// make sure we are enabled for notifications for the table
		SqlCacheDependencyAdmin.EnableTableForNotifications(connStr, "Authors");

		// this is case sensitive so make sure the first entry
		// matches the entry in the web.config file exactly
		// The first parameter here must be the connection string
		// NAME not the connection string itself...
        return new SqlCacheDependency("pubs", "Authors");
	}
	#endregion

	#region "14.11 Caching data with multiple dependencies"
    public void SetupCacheDependencies(string connStr)
    {
	    // make a dependency on the authory royalties file
	    // so if someone updates it, the cached data will 
	    // be disposed of 
	    string file = this.Server.MapPath("author_royalties.xml");
	    CacheDependency fileDep = new CacheDependency(file);

	    // use our method from 14.10 to make a SqlCacheDependency
	    SqlCacheDependency sqlDep = CreateSqlCacheDependency(connStr);

	    // set up data table to get
	    DataSet authorInfo = null;

	    // look for the pubs key in the cache
	    // If it isn't there, create it with a dependency
	    // on a SQL Server table using the SqlCacheDependency class.
	    if (this.Cache["authorInfo"] == null)
	    {
		    // the data wasn't there so go get it and put it in the cache
		    authorInfo = new DataSet("AuthorInfo");

            using (SqlConnection sqlConn = new SqlConnection(connStr))
            {
                using (SqlDataAdapter adapter =
                    new SqlDataAdapter("SELECT * FROM AUTHORS", sqlConn))
                {
                    adapter.Fill(authorInfo);

                    // now add the royalty info
                    authorInfo.ReadXml(file, XmlReadMode.InferSchema);

                    // make the aggregate dependency so that if either the
                    // db or file changes, we toss this out of the cache
                    AggregateCacheDependency aggDep = new AggregateCacheDependency();
                    // add the two dependencies to the aggregate
                    aggDep.Add(new CacheDependency[] { sqlDep, fileDep });

                    // add author info dataset to cache with the aggregate
                    // dependency so that if either changes the cache will refetch
                    this.Cache.Insert("authorInfo", authorInfo, aggDep);
                }
            }
	    }
	    else
	    {
		    authorInfo = (DataSet)this.Cache["authorInfo"];
	    }
    }
	#endregion

	#region "14.15 Inspect and change your web application configuration"
	public void TestConfig()
		{
			try
			{
				// Get the web.config file for this app
				System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration(@"/CSCBWeb");
				// Get the sqlCacheDependencySection
				SqlCacheDependencySection sqlCacheDep = (SqlCacheDependencySection)cfg.GetSection("system.web/caching/sqlCacheDependency");
				// create a database entry for the sql cache
				SqlCacheDependencyDatabase sqlCacheDb = new SqlCacheDependencyDatabase("pubs","LocalPubs",9000000);
				// add our database entry for the caching
				sqlCacheDep.Databases.Add(sqlCacheDb);
				// enable it
				sqlCacheDep.Enabled = true;
				// poll once a minute
				sqlCacheDep.PollTime = 60000;
				// save our new settings to the cfg file
				cfg.Save(ConfigurationSaveMode.Modified);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
		}

	#endregion

}