# Securing ASP.NET Core 3 with OAuth 2 and OpenID Connect

Referências de estudos:

Artigos:
- https://aaronparecki.com/oauth-2-simplified/#single-page-apps
- https://developer.okta.com/blog/2019/05/01/is-the-oauth-implicit-flow-dead
- https://tools.ietf.org/html/draft-parecki-oauth-browser-based-apps-02



Cursos:
- https://app.pluralsight.com/library/courses/securing-aspnet-core-3-oauth2-openid-connect/table-of-contents
- https://app.pluralsight.com/library/courses/authentication-authorization-aspnet-core/table-of-contents?aid=7010a000002BWqBAAW
- https://app.pluralsight.com/library/courses/openid-and-oauth2-securing-angular-apps/table-of-contents?aid=7010a000002BWqBAAW
- https://desenvolvedor.io/curso-online-fundamentos-de-identity-server/


---- 

Anotações: 



https://tools.ietf.org/html/rfc7519    -   JSON Web Token (JWT)
https://tools.ietf.org/html/rfc8725    -   JSON Web Token Best Current Practices
https://tools.ietf.org/html/rfc6749	   -   The OAuth 2.0 Authorization Framework
https://openid.net/specs/openid-connect-core-1_0.html  - OpenId


Contexto histórico: 

. 2005 - Saml 2p 
 	(As aplicações conseguiam se passar com você e ter seu login e senha, para resolver esses problemas as grandes empresas acabaram criando seus próprios padrões como Flick Auth, Google AuthSub, Yahoo!)

. 2007 - OAuth 1.0
 	Blaine Cook (Eng. Twiiter) deu inicio ao um protocolo open standard baseado nos esquemas criados pelas empresas. As empresas se uniram ao projeto.

 Final de 2007, crescimento do mobile com iPhone.

. OAuth 2.0 
	O objetivo era aproveitar o conhecimento da antiga implementação e adaptar para novos cenários, como mobile. 

	Houve conflitos nos drafts da RFC entre membros da comunidade e grandes empresas, para isso, foi usado termos vagos e retirados conceitos conflitantes e criando suas próprias RFC (Chamado de extensivéis).

 -------

 Estratégias de segurança para api:

* Credential sharing (depreciada):

	- Compartilhar nossas credenciais de acesso em troca de um beneficio Exemplo: Twitter, ferramentas que traçava seu perfil, publico e alcance.  

	Problemas:
		 - Senha salva em formato reversivel pelo serviço. 
		 - Nível de acesso completo.
		 - Impersonate ( O serviço se passa por você, não é possivel indentificar as ações do serviço)
		 - Aumenta o attack surface.
		 - Phishing. (Usar suas crendencias em outro serviço que não seja o original)

* Cookies:

	Vantangens: Verificar a segurança sobre um unico dominio (monolito)
	Problemas: 
		 - É enviado em todas as requisições, inclusive as desnecessárias.
		 - Cookie te da acesso completo a todas apis.
		 - Vuneravél Cross Site Request Forgery (CSFR / XSRF)
			Obs: É possivel fazer proteções com asp.net core e no webserver (nginx, apache..)

* JWT: 

	Problemas: 
		- Vulneravél a ataques XSS

	Estratégias de chaves de acordo com a arquitetura: 
		Chaves simetricas são indicadas em sistemas monoliticos.
		Chaves assimétricas em serviços distribuidos.

* API Keys 

	O usuário cria uma chave de api, para que o usuário tenha acesso a essa api.
	Uma boa integração entre sistemas B2B

	Problemas: 
		- A abordagem se torna um problema quando utilizada em aplicações que não conseguem proteger seus segredos, como em caso de SPAS. E seria facilmente roubada.

		- Tempo de expiração, como que as aplicações irão tratar isso? 

* OAuth 2.0 (Game change)

 	OAuth 2.0 é um framework de autorização para proteger apis http. Ele permite que o usuário delegue quais escopos as aplicações podera acessar, dessa forma o usuário se torna dono dos seus dados, por isso que ele é conhecido como um delegation protocol.
 
 	O Auth2 é formado por 4 papéis:

 		- Resource Owner: Uma pessoa ou entidade que fornece o acesso aos seus dados.

 		- Client: Geralmente é aplicação que iterage com o usuário ou um servidor em serviço do usuário. 

 		- Resource Server: Ela é a api exposta através do protocolo http e tem os dados do resource owner, para ter acesso ao seu conteudo é necessário um token do AuthServer

 		- AuthServer: Ele é o servidor OAuth2, ele é responsavel por emitir os tokens, conhecidos como bearer tokens. Detem informações do resource owner e expõe através de clains nos bearer tokens.

 -------

OpenID:

 	O OpenId Connect é uma camada de identidade simples que fica no topo do protocolo oAuth, ele habilita o client verificar a identidade do usuário final, com base na autenticação que foi realizada por um servidor de autorização (AuthServer). O OpenId obtem e expõe informações basicas de perfil do usuário, usando apis rest.

 	O OpenId é necessário porque embora o OAuth forneça autorização, ele não fornece autenticação. Na ótica do OAuth o usuário já se autenticou e provou que ele estava presente no AuthServer. 

 	O OAuth fornece um nivel de pseudo autenticação, ele fornece uma concessão, o AccessToken é uma licensa para acessar recursos que o usuário consentiu.

 	Através do OpenId, o OAuth Server fornece um token de idtoken. 
 	Esse idtoken é como se fosse um passaporte, fornecendo informações sobre o usuário 
 	em claims: custom claims, subject, authority, audience, Issue date, expiration date: 
	 	
	 	{
	 		"sub": "012u30123-123012931923-213124-41",
	 		"iss": "https://meuosso.com.br",
	 		"aud": "meu-client",
	 		"iat": 12312421424,
	 		"exp": 12321312321,
	 		"auth_time": 1231249241,
	 		"nonce": "12391820984029adja2e9u10941jda23814"
	 	}

	 O OpenId contem cerca de 20 claims padrões e 4 scopes para fornecer ao client ou api detalhes de usuário. Cada scope significa um subconjunto dessas claims.

	 O OpenId exige 3 endpoints sejam fornecidas para que o AuthServer e o client interagir: authorization_endpoint, token_endpoint, userinfo_endpoint

	 ----

	 O IdentityServer possui o OpenId e OAuth2 e não depende do Asp.net Identity, mas é comum utilizar a biblioteca para administrar os usuários. 

	 	* O identity server oferece controle de acesso para nossas apis.
	    * O asp.net identity é para gerenciar os usuários.

	 ----

	 Flows de autenticação: 

	 * Implicit flow (Depreciado) - https://openid.net/specs/openid-connect-core-1_0.html#ImplicitFlowAuth:

	 	Esse fluxo foi criado inicialmente para aplicativos baseados em browser.

	 	1. Em um determinado momento o usuário precisa se logar.
	 	
	 	2. Então o client redireciona para o AuthServer.
	 	
	 	3. O formato de autorização é o protocolo de OpenID, o AuthServer faz inicialmente validações para saber se o client é legitimo. Se for legitimo, aceita a solicitação e o client redireciona para o SSO.
	 	
	 	4. Nesse SSO, o usuário faz o login. (Aqui o openId Connect não define nenhum padrão)
	 	
	 	5. O AuthServer redireciona para o client, ele acopla o access token no get da chamada. 
	 	
	 	6. O client em posse desse access token, tem permissão de fazer acesso aos recursos. 

	 	Motivo da depreciação: 
	 		- Porque o access token é trafegado na chamada de volta, então o token pode ser pego facilmente, por firewall, histórico do usuário.


	* Authorization Code + PKCE (https://tools.ietf.org/html/rfc7636): 

	 É uma variação do implicit flow, a diferença nesse cenário ele possui uma camada a mais de segurança e o access token não é transmitido no retorno do AuthServer.  

	 Inicialmente foi previsto para aplicações nativas, além disso seu fluxo é perfeito para arquiteturas client-server.

	 	1. Em um determinado momento o usuário precisa se logar.
	 	
	 	2. O client gera code_verifier (string aleatorio de 47 a 128 char).

	 	3. Com o code_verifier ele faz um hash dele e assim gerado o code_challenge

	 	4. Então o client redireciona para o AuthServer junto com o code_challenge.

	 	5. O formato de autorização é o protocolo de OpenID, o AuthServer faz inicialmente validações para saber se o client é legitimo.

	    6. Se for legitimo, aceita a solicitação e o client redireciona para o SSO.
		
		7. O AuthServer, salva o code_challenge e gera um code.

	 	8.No SSO, o usuário faz o login. (Aqui o openId Connect não define nenhum padrão)
	 	
	 	9. Após o login realizado com sucesso, o AuthServer, retorna o usuário para o client, junto com o code.
	 	
	 	10. O cliente em posse desse code e o code_verifier e enviar para o AuthServer

	 	11. O AuthServer, pega o code_verifier, faz o processo de hash novamente e se for igual ao code_challenge anteriormente associado ao seu code, ele emite um access_token.

	 	12. O client em posse desse access token, tem permissão de fazer acesso aos recursos. 

		 	Objetivos desse flow é mitigar ataques de 
		 			- Referer headers.
		 			- Browser history
		 			- Authorization code replay


	Existem outros flows OpenId: 
		https://openid.net/specs/openid-connect-core-1_0.html#Authentication





