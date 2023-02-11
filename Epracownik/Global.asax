<%@ Application Language="C#" %>

<script runat="server">

void Session_Start(object sender, EventArgs e)
{
    Session["IsLoggedIn"] = false;
}


</script>