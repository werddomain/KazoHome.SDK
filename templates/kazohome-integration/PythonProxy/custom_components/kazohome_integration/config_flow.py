"""Config flow for KazoHome integration."""
from typing import Any

import voluptuous as vol
from homeassistant import config_entries
from homeassistant.core import HomeAssistant
from homeassistant.data_entry_flow import FlowResult

from .const import CONF_SOCKET_PATH, DEFAULT_SOCKET_PATH, DOMAIN


class KazoHomeConfigFlow(config_entries.ConfigFlow, domain=DOMAIN):
    """Handle a config flow for KazoHome."""

    VERSION = 1

    async def async_step_user(
        self, user_input: dict[str, Any] | None = None
    ) -> FlowResult:
        """Handle the initial step."""
        errors: dict[str, str] = {}

        if user_input is not None:
            return self.async_create_entry(
                title="KazoHome Integration",
                data=user_input
            )

        return self.async_show_form(
            step_id="user",
            data_schema=vol.Schema({
                vol.Optional(CONF_SOCKET_PATH, default=DEFAULT_SOCKET_PATH): str,
            }),
            errors=errors,
        )
