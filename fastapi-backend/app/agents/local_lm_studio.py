from langchain.agents import create_agent
from langchain_openai import ChatOpenAI
from app.tools.zendesk_python_client import check_order

class LMStudioChatAgent:
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
                 
        self.model = ChatOpenAI(
                    model=model,
                    temperature=temperature,
                    max_tokens=1000,
                    timeout=120,
                    base_url=base_url,
                    api_key=api_key
                )

    def get_agent(self):
        return create_agent(
                    self.model,
                    tools=[check_order],
                    system_prompt="You are a helpful assistant"
                )