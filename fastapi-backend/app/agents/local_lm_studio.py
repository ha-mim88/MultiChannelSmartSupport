from typing import List
from langchain_openai import ChatOpenAI
from langchain_core.language_models.chat_models import BaseChatModel
from langchain_core.messages import HumanMessage, SystemMessage, BaseMessage
from pydantic import BaseModel, Field

# Define Message first so it can be referenced by ChatRequest
class Message(BaseModel):
    """Defines the structure for a single chat message."""
    role: str = Field(..., description="The role of the message sender (e.g., 'user', 'system', 'assistant').")
    content: str = Field(..., description="The text content of the message.")


# Request/response models used by the FastAPI application
class ChatRequest(BaseModel):
    messages: List[Message]


class ChatResponse(BaseModel):      
    response: str


# --- 1. LLM Model Configuration Class ---

class LMStudioChatModel:
    """
    Encapsulates and manages a LangChain ChatOpenAI instance 
    configured to connect to a local LM Studio server.
    """
    
    # Default configuration matching the user's initial setup
    DEFAULT_BASE_URL = "http://localhost:1234/v1"
    DEFAULT_API_KEY = "lm-studio"
    DEFAULT_MODEL_NAME = "google/gemma-3-12b"
    DEFAULT_TEMPERATURE = 0.7
    DEFAULT_TOP_P = 1.0

    def __init__(self, 
                 base_url: str = DEFAULT_BASE_URL, 
                 api_key: str = DEFAULT_API_KEY, 
                 model: str = DEFAULT_MODEL_NAME, 
                 temperature: float = DEFAULT_TEMPERATURE, 
                 top_p: float = DEFAULT_TOP_P) -> None:
        """Initializes the LLM instance."""
        
        self._llm: BaseChatModel = ChatOpenAI(
            base_url=base_url,
            api_key=api_key,
            model=model,
            temperature=temperature,
            top_p=top_p
        )
        print(f"LM Studio Chat Model Initialized: {model} at {base_url}")


    def get_llm(self) -> BaseChatModel:
        """Returns the initialized ChatOpenAI instance."""
        return self._llm


# --- 2. Helper function for message conversion ---

def to_langchain_messages(messages: List[Message]) -> List[BaseMessage]:
    """
    Converts a list of Pydantic Message objects into a list of 
    LangChain's BaseMessage (HumanMessage, SystemMessage) objects.
    """
    lc_messages = []
    for msg in messages:
        role = msg.role.lower()
        if role == "user":
            lc_messages.append(HumanMessage(content=msg.content))
        elif role == "system":
            lc_messages.append(SystemMessage(content=msg.content))
        else:
            # Fallback for other roles (e.g., assistant, tool)
            lc_messages.append(BaseMessage(content=msg.content, role=msg.role))
    return lc_messages