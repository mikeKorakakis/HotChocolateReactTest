import { Button, Grid } from "@material-ui/core";
import userManager from "./oidc/userService";
import CallAPI from "./CallAPI";
import { useState, useEffect } from "react";
import Subscription from "./Subscription";

const Home: React.FC = () => {
	const [user, setUser] = useState<string>();
	useEffect(() => {
		userManager.getUser().then((user) => setUser(user?.profile?.name));
	}, []);

	return (
		<>
			<Subscription/>
			<h3 style={{textAlign:"center"}}>User: {user}</h3>
			<Grid container spacing={2} justify="center" direction="row">
				<Grid item>
					<Button
						variant="contained"
						size="large"
						color="primary"
						onClick={() => userManager.signinRedirect()}
					>
						Login
					</Button>
				</Grid>
				<Grid item>
					<CallAPI />
				</Grid>
				<Grid item>
					<Button
						variant="contained"
						size="large"
						color="secondary"
						onClick={() => userManager.signoutRedirect()}
					>
						Logout
					</Button>
				</Grid>
			</Grid>
		</>
	);
};

export default Home;
