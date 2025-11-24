from typing import List, Dict, Any
from E://ha-mim88/SmartSupport/fastapi-backend/app/agents/local_lm_studio import LMStudioChatAgent
import json
import traceback

llm_model = LMStudioChatAgent()
agent = llm_model.get_agent()

result = agent.invoke( 
            {"messages": [{"role":"User","content":"hi"}]} ,
            context={"user_role": "helpful expert assistant."}
        )
print(result)