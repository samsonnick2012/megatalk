openfire
========

Fork openfire 3.8.2

Install
===

git clone https://github.com/jiangrongyong/openfire.git

git submodule update

cd build

ant plugins

ant openfire

chmod +x target/openfire/bin/openfire.sh

target/openfire/bin/openfire.sh start
