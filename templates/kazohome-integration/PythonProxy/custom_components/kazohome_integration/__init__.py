"""KazoHome Integration for Home Assistant - Python Proxy"""
from .const import DOMAIN

async def async_setup(hass, config):
    """Set up the KazoHome integration."""
    return True

async def async_setup_entry(hass, entry):
    """Set up KazoHome from a config entry."""
    from .integration import KazoHomeIntegrationProxy
    
    integration = KazoHomeIntegrationProxy(hass, entry)
    
    if not await integration.async_setup():
        return False
    
    hass.data.setdefault(DOMAIN, {})
    hass.data[DOMAIN][entry.entry_id] = integration
    
    return True

async def async_unload_entry(hass, entry):
    """Unload a config entry."""
    integration = hass.data[DOMAIN].get(entry.entry_id)
    
    if integration:
        await integration.async_unload()
        hass.data[DOMAIN].pop(entry.entry_id)
    
    return True
