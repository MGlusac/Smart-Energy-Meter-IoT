from flask import Flask, render_template
import requests

app = Flask(__name__)
@app.route('/')
def home():
    return render_template("home.html")

@app.route('/about/')
def about():
    return render_template("about.html")

if __name__ == "__main__":
    app.run(debug=True)


#r = requests.get('https://api.github.com/user', auth=('user', 'pass'))
#r.status_code
#200
#r.headers['content-type']
#'application/json; charset=utf8'
#r.encoding
#'utf-8'
# r.text
#u'{"type":"User"...'
#r.json()
#{u'private_gists': 419, u'total_private_repos': 77, ...}

#GET https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}?api-version=2017-05-10
