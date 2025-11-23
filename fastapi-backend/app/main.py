from typing import List
from fastapi import FastAPI, Request, HTTPException
from .agents import (
    LMStudioChatModel,
    to_langchain_messages,
    ChatRequest,
    ChatResponse,
    Message
)

app = FastAPI()

@app.get("/ping")
def ping():
    return {"message": "pong"}

@app.post("/api/chat", response_model=str)
async def chat(request: List[Message]):
    """
    Accepts a list of chat messages and returns an LLM response 
    by calling the local LM Studio endpoint.

    Raises:
        HTTPException 503: If the LLM client failed to initialize (LM Studio not running).
        HTTPException 500: If the LLM invocation fails during the call.
    """
    # Initialize the LM Studio Chat Model
    llm_model = LMStudioChatModel()
    llm_client = llm_model.get_llm()

    if not llm_client:
        # Check for initialization failure at the start
        print("ERROR: LLM Client is not initialized.")
        raise HTTPException(
            status_code=503, 
            detail="LLM service is unavailable. Check LM Studio server configuration."
        )

    try:
        # 1. Convert incoming Pydantic messages to LangChain message format
        lc_messages = to_langchain_messages(request)
        
        # 2. Asynchronously invoke the LLM via the imported client
        # This is where the network request to http://localhost:1234/v1 is made.
        response = await llm_client.ainvoke(lc_messages)

        # 3. Return the LLM's response content
        return response.content

    except Exception as e:
        # Log the specific error for server-side debugging
        import traceback
        print(f"\n--- LLM Invocation Error ---\n")
        print(f"Request: {request.model_dump_json(indent=2)}")
        print(f"Traceback: {traceback.format_exc()}")
        print(f"\n----------------------------\n")

        # Return a generic 500 error to the client
        raise HTTPException(
            status_code=500, 
            detail=f"Error communicating with the local LLM. Check LM Studio server status. Detail: {type(e).__name__}"
        )