# H2-Projekt {Indsæt gruppenavn}
# NeonTech: Brugernavn - h2gruppe5online@gmail.com , Password - 2^iiy6E57Jn9JcVeVbHYd^#


Her skal alt omkring jeres projekt være. Alle jeres OOP klasser, Diagrammer (UML, Database, mm.), Blazor-kode og API-kode. 
Projektet han findes her - [H2 Projekt forløb på Notion](https://mercantec.notion.site/Projekt-H2-Booking-side-33e086a54fd84630b2c63bd67a5066d2?pvs=4)

Det er delt op i 5 mapper 

## [Blazor](https://github.com/MAGS-Template/H2-Projekt/tree/main/Blazor)
Her er størstedelen af jeres projekt, her har vi alt UI. 
Blazor versionen er nu opdateret til .NET 8.0 som er LTS

## [Domain Models](https://github.com/MAGS-Template/H2-Projekt/tree/main/DomainModels)
Her er alle jeres klasser, som I skal bruge til jeres Blazor og API. 
Domain Models / Class Libary versionen er nu opdateret til .NET 8.0 som er LTS

## [Dokumentation](https://github.com/MAGS-Template/H2-Projekt/tree/main/Dokumentation)
Mappen her er stortset tom, fordi I selv skal udfylde den med jeres dokumentation fra jeres projekt! Der skal være jeres UML diagram, enten bare det nyeste eller alle versioner. 
Jeres Database diagram som i har lavet med [DrawSQL.app](drawsql.app) eller andet program.

## [API](https://github.com/MAGS-Template/H2-Projekt/tree/main/API)
Her er jeres API, den bruger vi til at forbinde sikkert til vores database og for at fodre data til vores Blazor Projekt.
ASP.NET Core Web API versionen er nu opdateret til .NET 8.0 som er LTS - Vi bruger dog <strong>ikke</strong> Native AOT versionen med ahead-of-time!

## [SQL-Scripts](https://github.com/MAGS-Template/H2-Projekt/tree/main/SQL-Scripts)
Vi skal skrive scripts som kan queries mod vores database som enten er hostet lokalt eller på en cloudplatform! Det er vigtigt at gemme dem, så vi bruger mappen her og gemmer dem som .SQL filer. De kan eksekveres med mange GUI's - personligt anbefaler jeg [SQLTools](https://www.notion.so/mercantec/VSCode-Extensions-f4e03a6568ee483f85d9fc018ba6baa7?pvs=4#e439f568d1fe4749afa04ee204f37ac9) som er en udvidelse til VSCode. [TablePlus](https://tableplus.com/) og [HeidiSQL](https://www.heidisql.com/) er også gode bud!

### Hosting
Vi kigger på Hosting under H1 forløbet, men vores applikation her burde gerne være live på [Render.com - Blazor](https://h2blazor.onrender.com/) og [Render.com - API](https://h2api.onrender.com/swagger/index.html)! *Siden bliver inaktiv efter lidt tid, så hvis den ikke loader, er [containeren](https://www.notion.so/mercantec/Containers-a9c3613888d342cca0221c7e0f68a767) ved at starte op!*

