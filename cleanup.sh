#!/bin/bash

find . -iname "bin" -o -iname "obj" -o -iname "artifacts" | xargs rm -rf
