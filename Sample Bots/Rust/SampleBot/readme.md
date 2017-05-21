Rust Bot
========

Setup
-----

Install Rust using the instructions from
https://www.rust-lang.org. Add the binary directory (it should contain
an executable called "cargo") to the path if the installed hasn't done
so already.

Compiling
---------

Do a release mode build with Cargo.

`cargo build --release`

This will also download the packages specified in `Cargo.toml`. The
binary is placed in `target/release/rust_sample_bot`.

Windows / Linux Nonsense
------------------------

Depending on the platform you're running on, you may need to open up
`bot.json` to add or remove the '.exe' extension from the RunFile.
