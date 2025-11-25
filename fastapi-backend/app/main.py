# app/main.py
from typing import List, Dict, Any
from fastapi import FastAPI, HTTPException
from app.agents import LMStudioChatAgent
import json
import traceback

app = FastAPI(title="Smart Support AI Backend")

@app.post("/")
async def home():
    return {"message": "hi, how can i help !?!"}

@app.get("/ping")
def ping():
    return {"message": "pong"}


@app.post("/api/chat", response_model=str)
async def chat(messages: List[Dict[str, str]]):
    try:
        llm_model = LMStudioChatAgent()
        agent = llm_model.get_agent()

        result = agent.invoke( 
                    {"messages": messages} ,
                    context={"user_role": "helpful expert assistant."}
                )
        ai_message_content = result['messages'][-1].content
        return ai_message_content
    except Exception as e:
        print("LLM Error:", traceback.format_exc())
        raise HTTPException(status_code=500, detail=f"LLM Error: {str(e)}")