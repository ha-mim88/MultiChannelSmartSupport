"""Agents package for LLM adapters and helpers.

Expose stable symbols from local_lm_studio for convenient imports.
"""

from .local_lm_studio import (
    LMStudioChatModel,
    to_langchain_messages,
    ChatRequest,
    ChatResponse,
    Message,
)

__all__ = [
    "LMStudioChatModel",
    "to_langchain_messages",
    "ChatRequest",
    "ChatResponse",
    "Message",
]
