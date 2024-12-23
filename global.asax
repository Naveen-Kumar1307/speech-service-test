<%@ Application Language="C#" Debug="true" %>

<script runat="server">


	void Application_BeginRequest(object sender, EventArgs e)
	{
		if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
		{
			HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
			HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
			HttpContext.Current.Response.End();
		}
	}

</script>