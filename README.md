DEV-BlOG

Dev-Blog is a dynamic web application that allows users to register, read, like, and comment on posts. Additionally, blog posters can register as admins, enabling them to add and edit their blog posts based on their technical expertise. As a technical blog post site, we have incorporated tags to categorize posts by technology, making it easy for users to find content related to their interests.

# This workflow will build a .NET project
# For more information see: https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net

name: .NET

on:
  push:
    branches: [ "master" ]
  pull_request:
    branches: [ "master" ]

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 7.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
