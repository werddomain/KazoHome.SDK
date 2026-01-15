"""Integration proxy that communicates with the .NET backend."""
import asyncio
import logging
from typing import Any

from homeassistant.config_entries import ConfigEntry
from homeassistant.core import HomeAssistant

from .const import CONF_SOCKET_PATH, DEFAULT_SOCKET_PATH

_LOGGER = logging.getLogger(__name__)


class KazoHomeBridge:
    """Bridge for communicating with .NET backend."""

    def __init__(self, socket_path: str):
        self._socket_path = socket_path
        self._reader = None
        self._writer = None
        self._connected = False

    async def connect(self) -> bool:
        """Connect to the .NET backend."""
        try:
            self._reader, self._writer = await asyncio.open_unix_connection(
                self._socket_path
            )
            self._connected = True
            _LOGGER.info("Connected to KazoHome .NET backend at %s", self._socket_path)
            return True
        except Exception as ex:
            _LOGGER.error("Failed to connect to .NET backend: %s", ex)
            return False

    async def disconnect(self) -> None:
        """Disconnect from the .NET backend."""
        if self._writer:
            self._writer.close()
            await self._writer.wait_closed()
        self._connected = False
        _LOGGER.info("Disconnected from KazoHome .NET backend")

    @property
    def is_connected(self) -> bool:
        """Return connection status."""
        return self._connected


class KazoHomeIntegrationProxy:
    """Proxy for the KazoHome integration."""

    def __init__(self, hass: HomeAssistant, entry: ConfigEntry):
        self.hass = hass
        self.entry = entry
        socket_path = entry.data.get(CONF_SOCKET_PATH, DEFAULT_SOCKET_PATH)
        self._bridge = KazoHomeBridge(socket_path)

    async def async_setup(self) -> bool:
        """Set up the integration."""
        return await self._bridge.connect()

    async def async_unload(self) -> bool:
        """Unload the integration."""
        await self._bridge.disconnect()
        return True
