use dotenv::dotenv;
use rocket::serde::{json::Json, Deserialize};
use rocket::{post, routes};

#[tokio::main]
async fn main() -> Result<(), rocket::Error> {
    dotenv().ok();
    println!("Hello, world!");
    rocket::build()
        .mount("/", routes![throw_dice])
        .launch()
        .await?;

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
