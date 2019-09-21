#!/bin/bash

faas-cli build -f csharp-kestrel-echo.yml
faas-cli push -f csharp-kestrel-echo.yml
faas-cli deploy -f csharp-kestrel-echo.yml

