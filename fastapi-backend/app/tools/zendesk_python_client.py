from langchain.tools import tool
from langchain.agents import create_agent

@tool
def check_order(order_id: str) -> str:
    """
    Look up order status by order number. 
    Use when the user mentions order, tracking, delivery, or a specific order number.
    """
    
    try:
        mock_orders = {str(i): f"Order {i}: iPhone 15 Pro, shipped Nov {i-100}, arriving soon." 
                      for i in range(123, 143)}
        
        if order_id in mock_orders:
            result = f"✅ Order {order_id} found!\n{mock_orders[order_id]}\nTracking: https://nova.tech/track/{order_id}"
            return result

        return f"❌ No order found with ID {order_id}. I've created a support ticket for you."
    
    except Exception as e:
        return f"Error checking order {order_id}: {str(e)}"