use dotenv::dotenv;
use rocket::futures::{FutureExt, TryFutureExt};
use rocket::serde::{json::Json, Deserialize};
use rocket::{post, routes};
use poise::serenity_prelude::{self as serenity, ChannelId};
use tokio::join;

struct Data {
    answer_channel: Option<ChannelId>,
}

type PoiseError = Box<dyn std::error::Error + Send + Sync>;
type PoiseContext<'a> = poise::Context<'a, Data, PoiseError>;

#[poise::command(slash_command)]
async fn here(ctx: PoiseContext<'_>) -> Result<(), PoiseError> {
    ctx.say("I see you").await?;
    Ok(())
}

#[tokio::main]
async fn main() -> Result<(), rocket::Error> {
    dotenv().ok();
    println!("Hello, world!");
    let rocket = rocket::build()
        .mount("/", routes![throw_dice])
        .launch();

    let token = std::env::var("DISCORD_TOKEN").expect("missing DISCORD_TOKEN");

    let intents = serenity::GatewayIntents::non_privileged();

    let framework = poise::Framework::builder()
        .options(poise::FrameworkOptions {
            commands: vec![here()],
            ..Default::default()
        })
        .setup(|ctx, _ready, framework| {
            Box::pin(async move {
                poise::builtins::register_globally(ctx, &framework.options().commands).await?;
                Ok(Data {
                    answer_channel: None,
                })
            })
        })
        .build();

    let mut client = serenity::ClientBuilder::new(token, intents)
        .framework(framework)
        .await
        .unwrap();

    let bot = client
        .start();

    _ = join!(rocket, bot);

    Ok(())
}

#[derive(Deserialize)]
#[serde(crate = "rocket::serde")]
struct DiceThrow<'r> {
    throw: &'r str,
}

#[post("/dice", data = "<dice_throw>")]
fn throw_dice(dice_throw: Json<DiceThrow<'_>>) {
    println!("received dice throw: {}", dice_throw.throw);
}
