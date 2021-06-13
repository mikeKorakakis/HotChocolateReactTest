import { useQuery, gql, useLazyQuery } from "@apollo/client";
import { Button, Grid } from "@material-ui/core";
import CustomCard from "./ui/Card";

const USERS = gql`
query Users {
    users {
        id
        email
        firstName
        lastName
    }
}
`;
const CallAPI: React.FC = () => {

	const [getUsers, { loading, error, data }] = useLazyQuery(USERS);
	if (loading) return <p>Loading...</p>;
	if (error) return <p>{error.message} :(</p>;
        if(data) console.log(data)
	return (
		<div>
            <Button variant="contained" size="large" style={{backgroundColor: "green", color: "white"}} onClick={() => getUsers()}>API</Button>
            <Grid justify="center" container spacing={4}  style={{marginTop: 10, position:"absolute", left:10}}>
        	{data?.users.map(({ id, firstName, lastName, email }: any) => (
				<Grid item  key={id}>
					<CustomCard email={email} firstName={firstName} lastName={lastName}></CustomCard>
				</Grid>
			))}
            </Grid>
		</div>
	);
};

export default CallAPI;
