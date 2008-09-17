<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="DefaultPage" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
   <meta http-equiv="Pragma" content="no-cache" />
   <meta http-equiv="Expires" content="-1" />
   <title>Windows Live ID&trade; Web Authentication Sample</title>
   <style type="text/css">
      table
      {
         font-family: verdana;
         font-size: 10pt;
         color: black;
         background-color: white;
      }
      h1
      {
         font-family: verdana;
         font-size: 10pt;
         font-weight: bold;
         color: #0070C0;
      }
   </style>    
</head>

<body><table width="320"><tr><td>
    <h1>Welcome to the C# Sample for the Windows Live&trade; ID Web
    Authentication SDK</h1>

    <p>The text of the link below indicates whether you are signed in
    or not. If the link invites you to <b>Sign in</b>, you are not
    signed in yet. If it says <b>Sign out</b>, you are already signed
    in.</p>

    <iframe 
       id="WebAuthControl" 
       name="WebAuthControl"
       src="http://login.live.com/controls/WebAuth.htm?appid=<%=AppId%>&style=font-size%3A+10pt%3B+font-family%3A+verdana%3B+background%3A+white%3B"
       width="80px"
       height="20px"
       marginwidth="0"
       marginheight="0"
       align="middle"
       frameborder="0"
       scrolling="no">
   </iframe>

<p>
<% if(UserId == null) { %>
      This application does not know who you are! Click the <b>Sign in</b> link above.
<% } else { %>
      Now this application knows that you are the user with ID = "<b><%=UserId%></b>".
<% } %>
</p>

</td></tr></table></body>
</html>
