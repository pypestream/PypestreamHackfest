# Caap2017


 
## Ascend Hackfest - Microsoft Bot Framework, Hackfest week
### March 6-10th 2017, Microsoft building 25, East Campus, Redmond, WA 

##  Attendees
    Grant Steinfeld ( Pypestream, Lead Software Architect )
    Steve Kurz  (Microsoft Sales, Customer Experience CE )
    Lucas Huet-Hudson (Microsoft, Programmer, NodeJS )
    Richard diZerga (Microsoft Senior Programmer C#, Active Directory specialist)




## Objectives:
   > Prove that a Microsoft(MS) Active Directory(AD) on-premise can indeed by wired up to Azure AD and MS Graph API
   > Evaluate at a deep level, The MS Bot building framework
   > Research cogintive services such as Sentiment, NLP, Translation and facial recognition
   > Expose at a high level to MS about the Pypestream Bot framework   


### Summary of findings
 
    All in all Microsoft has a solid and well thought out Bot framework.  

    Weaknesses:  Code heavy ( no external representation like Graph or CSV ) indicates reliance on C# / Node programmers and slow going
          In addition, due the async nature of closure in C#, also difficult to debug these cases.

    Stengths:  Active Directory Connector works as advertised.  It connects to Azure AD and we are able to demonstrate that connection.
    Graph API allows up to update AD data entries on the Azure cloud as well as writeback to on-premis AD.
    Large team developing: There are maybe 100 developers on the core team, and also all the resources out of cognitive services (LUIS)

    At this time I would not recommend Pypestream utilize their bot framework, but keeping an eye on it was helpful to understand the bigger picture.  Adoption of the Azure Cloud is a good idea, especially give the API access to Identity in AD, as well as wiring some very compeling cognitive services, for instance Entity extraction, Facial recognition and translation.  These were tested and were quite easy to integrate.  There are numerous other
    Services worth looking at, these include Sentiment, which very sadly we did not have time to explore.

    Microsoft appears very interested in Pype-bots and a lot of the afternoon of Day one was spent on explaining in detail what technology our bots are made of.




## Detailed, day by day account

### Day one
Upon arrival just after noon lunch, met the dedicated MS team and discussed  the bot stack we should focus on.  
Options were Python vs Node vs C#.  Finally after much deliberation, we decided on C# for various reasons, not including Richard diZerga literacy with C#, Azure AD and besides the MS Visual Studio editor been the far superior IDE of choice.
Furthermore we discussed the aspects of MS Bot architecture and compared and contrasted it with Pypestream’s bDiscussed goals for the hackfest in general.
Looked at Azure REST integration wrt our connector needs.
Setup AD Connect on Steve Kurz local “AD domain controller VM” running in Azure.   We connected Azure AD directory.
We added a new Azure user in the “Tulum” directory with a user Grant T Steinfeld.  We were able to show on-premise AD contacts show up in Azure AD

Interpreted  the Pypestream AD bot flow provided Eugene.  Richard made various comments and criticisms:
discussed the difficult aspects around password reset
Proxy/impersonation as a service
Managerial consent workflow
Put up an intial project plan detailing the goals and spikes for the upcoming week.


### Day two
Discussed further Azure AD wrt with programatic/daemon with Popup to MSAuth – cleared with Jatin

Refined our planned sprints for week
Started building C# bot flow for MS bot .10% complete.. photo bot flow of wb to follow

Flexed using Richard’s wrapper to MS bot fwk ( https://github.com/richdizz/BotAuth ) 
looked at Graph API explorer to see REST calls into Azure AD (Steve)


### Day three
Morning started with 8:30 breakfast on campus, and met with one of the most animated people I have ever met.  His name is Dr Neil Rooyden and we chatted around the theories and concepts around  Identity.  He proceeded to demonstrate Windows Hello 3d face detection technology as a better and faster option to passwords

With Richard, diZerga, continued building C# bot flow for MS bot 
At only 25% complete as we ran into difficulty with deeply nested dialog flows.  This indeed was one of the downsides of the MS Bot Framework. In addition, building bots with their framework is expressed entirely in ‘code’.  The code relies heavily on async and hard to follow anything more than a naïve model.   Even the best expert coder from MS appeared to struggle with building bots.
They liked the idea of translation between our graph representation and the idea of graph flow that translates.  One key differentiator is MS uses a stack for their runtime data structure not a DAG ( side note Richard diZerga and myself discussed this March 15th in a follow up meeting and he also agrees a DAG is preferable for the design time representation )

Lucas showed some NodeJS examples and we spent half an hours looking at NodeJS.  I was impressed at the simplicity of the code when compared to the C# In particular the cascading style and closure aspects of NodeJS seem to drive this along. https://docs.botframework.com/en-us/node/builder/overview/


Lunchtime today , we ate delicious pizza at the awesome MS building 18 across the road from our venue ( Building 25 ).  It was here I later bumped into and had very technical discussions with the head of the DX Visual Studio partner, Amanda Silver ( https://www.linkedin.com/in/amanda-silver-a155701/ ).  She knows all the top language dignatories, including Anders Hejlsberg, Erich Gamma and Erik Meyers to name a few.  She will be an important contact to persue.

During the afternoon Steve and myself looked at scraping PS FAQ and how to connect it to  LUIS Language Understanding Intelligent Service
o	AI and ML is what Bing and Cortana run on.
o	Heavily based off of natural language and programming by example from MS Research
At the end of the Day we decided to drop spike one


### Day four 
continued building C# bot flow for MS bot - wire in password reset with Graph API
and resolve bot building headaches and see if there are patterns/best practices. possible ideas  for boiler plating codegen

see if NodeJS ( Lucas ) can writed similar bot to the C# on quicker and easier to grok

look at cognitive API's we can consume
   voice to text
facial recognition 
LUIS NLP faq Azure table option to make bots more dynamic ( ie table changes   intent and responses ) (Steve)



### Day five
On the last day of the hackfest, we got in early to finish any last minute bug fixes and to
grok what Richard, developed last eveing.  Basically he refined the change Azure AD profile image, to present a scenario not unlike Winbdows Hello.

At 9:30 I presented the end result of the Hackfest.  Basically the demo** went really well and the detailed demo script and video can be referenced below.

The presentation consisted of a brief introduction, then a show and tell of the demo.
There was mixed enthusiam in the audience, consisting around 50 Microsfot folk.  Some good
questions arose as well as some request to MS regarding improving their framework.  They asked if
we would adopt their framework and I passed on answering and told them Humphrey or Jatin could answer that question.  They did surrender a piece of information about an alpha JSON to Bot Dialog flow code generation module they were working on.

We recieved a good thanks and that ended the day.  Stainless steel water bottles were handed out emblazoned with 'Kiss my Bot'


### Refernces

** The demo script

Created a video overview ( Richard) of what is demoing the bot and how to publish the bot to channels using something called direct line to Facebook, Slack, Skype etc
Ran the  demo from the bot emulator:
 
Part 1
1)       Start the project/debugger  
2)       Open Emulator and connect to the bot endpoint
3)       Wait for bot to proactively message the emulator
4)       Type I’m locked out
5)       When prompted for an alias enter david@richdizz.com (The mobile number points Grant’s mobile number)
6)       Show the code sent to the mobile device
7)       Enter the code incorrectly (perhaps talk about limiting the incorrect codes)
8)       Repeat 4-7 but using the correct code
9)       NOTE: the code to perform the actual reset is in place, but I did not admin-consent the app so I wouldn’t try logging in with david@richdizz.com as it won’t really change the password right now

Part 2
1)       Open Graph Explorer at https://developer.microsoft.com/en-us/graph/graph-explorer and sign-in with kirk@richdizz.com and password pass@word1
2)       Run the query https://graph.microsoft.com/v1.0/me/photo/$value 
3)       Type change my profile picture
4)       When prompted to sign-in, use kirk@richdizz.com with password pass@word1 (be careful about browser cookies…perhaps have incognito window up)
5)       When prompted to upload a new profile photo, use a picture that isn’t Kirk (discuss the cognitive services preventing photo quality issues in directory)
6)       Type change my profile picture a second time
7)       This time select a photo of Kirk (also on thumb drive), but not the same one that is currently uses
8)       Go back to Graph Explorer and re-run the REST query to show the updated photo

Part 3
1)       Perform a random query (ex: What is the height of a basketball rim)
2)       Show the federated search in Markdown format.



## Repos
<< to be listed >>

## References
repo for the graph API console
https://github.com/Azure-Samples/active-directory-dotnet-graphapi-console/blob/master/GraphConsoleAppV3/AuthenticationHelper.cs

