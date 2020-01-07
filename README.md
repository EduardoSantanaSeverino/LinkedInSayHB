# LinkedInSayHB

# Purpose: 
This tool was developed with the initiative of sending a Happy Birthday greeting to your connections on LinkedIn.

# Variables: 
The application will request to connect to your LinkedIn account, for that purpose, you need to put your **User (email)**, **Password**, and if add a **custom message**. If you leave that field Blank the greeting will look like:
> Happy birthday! <Contact_Firsts_name_> Best wishes!

If you add a personal message the only section that may be edit should be "Best wishes".
So if you add, as a personal message: "I hope you have a wonderful day."
The message will be:
> Happy birthday! <Contact_Firsts_name_> I hope you have a wonderful day.

# How to use it: 
First, run the App from your CMD console:
> dotnet LinkedInSayHB.dll Param1 Param2 Param3

Where
**Param1:** the console will expect the first parameter, which is the email account that we will use to connect.
**Param2:** the console will expect the second parameter, which is the password of your email. 
**Param3:** By leaving in blank, you will keep the default message "Best wishes" at the end. and if you add a message instead that the one by default.
The application won't support the following symbols: ' " ,
__The application will only open a window on your browser and connect to Linkedin, the application will not record any data that you input. so your password will be secure.__
