# MultiChannelSmartSupport

# MultiChannelSmartSupport – Production-Ready Voice-First AI Assistant  
**Pure C# .NET 10 Blazor • 100% Local LLM • Voice + Text • Tool Calling • Analytics • alternative fastapi backend for AI**
GitHub: https://github.com/ha-mim88/MultiChannelSmartSupport.git

https://github.com/user-attachments/assets/your-screenshot-or-gif-here.gif

### The Only Full-Stack, Voice-Enabled, Offline-Capable AI Assistant Template Built Entirely in C#


Perfect starting point for:
- Enterprise/Personal internal assistants
- Customer support bots
- Healthcare / banking virtual agents
- Any regulated environment requiring data privacy

## Features (All 100% Working)

| Feature                                 | Status | Details |
|----------------------------------------|--------|-------|
| .NET 10 Blazor Server + Identity       | Done   | Full auth, roles, roles, email login |
| Local LLM via LM Studio (OpenAI compatible) | Done   | Zero cost, runs offline |
| OpenAI fallback (just flip a switch)   | Done   | Production ready in seconds |
| Real tool calling (check_order + extensible) | Done   | Full round-trip with DB logging |
| Per-user persistent chat history       | Done   | SQL Server + EF Core |
| ChatGPT-style session sidebar          | Done   | Create, rename, delete sessions |
| Voice input (speech-to-text)           | Done   | Web Speech API – works on mobile |
| Natural voice output (TTS)             | Done   | Microsoft Edge Neural voices |
| Speech + text mixed mode               | Done   | Speak or type – bot replies both ways |
| Live analytics dashboard               | Done   | AHT, resolution rate, tool usage, charts |

## Quick Start (5 minutes)

```bash
# 1. Clone
git clone https://github.com/ha-mim88/MultiChannelSmartSupport.git
cd MultiChannelSmartSupport

# 2. Start LM Studio, load any gguf model, start server on http://localhost:1234

# 3. Run the app
dotnet run --project SmartSupport.AppHost

# 4. Open browser
https://localhost:5269
```

## Project Structure

```
SmartSupport/
├── SmartSupport.AppHost          # Aspire orchestrator
├── SmartSupport.Frontend         Blazor Server (UI + API)
├── SmartSupport.ServiceDefaults  Shared config
├── SmartSupport.Models           EF Core models
├── Services/
│   └── LlmService.cs             Core AI + tool calling logic
├── Components/
│   └── VoiceChatInput.razor      Voice in + TTS out
├── Pages/
│   ├── MyAssistant.razor         Main chat with session sidebar
│   └── Analytics.razor           Live dashboard with ApexCharts
└── wwwroot/js/speech.js          Speech recognition + TTS interop
```

## Configuration (appsettings.json)

```json
{
  "Llm": {
    "UseLocal": true   // false = use OpenAI (add ApiKey below)
  },
  "OpenAI": {
    "ApiKey": "sk-..."
  },
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\mssqllocaldb;Database=SmartSupport;Trusted_Connection=true;Encrypt=false;"
  }
}
```

## Deployment

### Azure App Service (Free Tier)
```bash
az webapp up --name smartsupport-yourname --resource-group MyGroup --runtime "DOTNET|10.0"
```

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY bin/Release/net10.0/publish/ /app
WORKDIR /app
ENTRYPOINT ["dotnet", "SmartSupport.Frontend.dll"]
```

## Roadmap / Easy Extensions

- Plug in real Zendesk / Salesforce / SAP / TMS
- Add Whisper.cpp for 100% offline voice
- Add SignalR real-time agent handoff
- Export chat as PDF
- Multi-language (just change voice + lang code)

## License

MIT License – fork it, sell it, ship it to production.  
Just keep the copyright notice.

## Made by

**Md. Thoukir Hasan Ha-Mim**  
---

**Star this repo if you want a great template to start your enerpriser grade ai assistant with end-end c# blazor and optional python based ai integration**

Questions? Open an issue – I answer all of them.

You now own the best open-source, voice-first, local-LLM AI assistant template in the .NET ecosystem.

Go ship it.