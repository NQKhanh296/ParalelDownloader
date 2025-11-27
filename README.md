# Paralelní Downloader (C# / .NET)

Tento projekt je konzolová aplikace, která umožňuje paralelní stahování více souborů najednou.  
Aplikace automaticky zvolí počet paralelních vláken podle počtu procesorových jader a počtu URL, které má stahovat.

## Funkce
- Paralelní stahování pomocí thread poolu (řízeno přes `SemaphoreSlim`)
- Návrhové vzory:
  - Singleton (`HttpClientSingleton`)
  - Factory Method (`WorkerFactory`)
  - Producer–Consumer (`TaskProducer` + `DownloadWorker`)
- Ukládání konfigurované cílové složky do textového souboru
- Validace cesty (musí být existující a zapisovatelná)
- Uživatelské menu:
  1. Stahování URL
  2. Změna složky pro ukládání
  3. Ukončení programu
- Automatické určování přípony souboru podle URL
- Počítání úspěšně stažených souborů
- Zobrazení výsledné statistiky

## Jak spustit projekt
1. Ve složce **/output** najděte soubor:

   **ParalelDownloader.exe**
2. Stahujte si tento soubor
3. Spusťte ho dvojklikem.

4. V konzoli si vyberte akci:
   - stahovat URL
   - změnit složku
   - ukončit program

## Autor
Nguyen Quoc Khanh
