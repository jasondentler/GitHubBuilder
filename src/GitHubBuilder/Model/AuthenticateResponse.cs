﻿namespace GitHubBuilder.Model
{

    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Token { get; set; }
    }

    /*
{
"id": 1,
"url": "https://api.github.com/authorizations/1",
"scopes": [
"public_repo"
],
"token": "abc123",
"app": {
"url": "http://my-github-app.com",
"name": "my github app"
},
"note": "optional note",
"note_url": "http://optional/note/url",
"updated_at": "2011-09-06T20:39:23Z",
"created_at": "2011-09-06T17:26:27Z"
}
     */
}
