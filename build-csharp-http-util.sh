#!/bin/bash

faas-cli build -f csharp-http-util.yml
faas-cli push -f csharp-http-util.yml
faas-cli deploy -f csharp-http-util.yml
