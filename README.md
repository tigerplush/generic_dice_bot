# generic_dice_bot
This is a bot with an API to manage dice throws in Discord.

## Usage
Either use the `/throw` command from discord or do a `POST-Request` to `/api/dice`.
The api consumes a `JSON` and responds with a dice result. If a channel was configured with the `/here` command within Discord, the dice result will also be posted in the configured channel.

```json
{
    "diceThrow": "string",
    "requester": "optional string",
    "reason": "optional string",
    "imageUrl": "optional string"
}
```

The dice throw is a "standard" dice string, e.g. "3d6" will throw three six-sided die. "1d20+10" will throw one 20 sided die and add ten. Subtractions are also supported.

When the string is invalid, the API and the `/throw` command will respond with the found errors.

The requester is an optional string that will be used to flourish the dice throw embed in Discord (essentially the author of an embed).

The reason is an optional string that will be used to flourish the dice throw embed in Discord.

The image url is an optional string, that will flourish the dice throw embed in Discord (essentially the icon url of an embed).